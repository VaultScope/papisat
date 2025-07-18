using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VaultScope.Core.Interfaces;
using VaultScope.Core.Services;
using VaultScope.Infrastructure.Data;
using VaultScope.Infrastructure.Data.Repositories;
using VaultScope.Infrastructure.Http;
using VaultScope.Infrastructure.Reporting;
using VaultScope.Security.Detectors;
using VaultScope.Security.Validators;

namespace VaultScope.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Data Source=vaultscope.db;Cache=Shared";
        
        // Use connection pooling for better performance
        services.AddDbContextPool<VaultScopeDbContext>(options =>
        {
            options.UseSqlite(connectionString);
            options.EnableSensitiveDataLogging(false);
            options.EnableServiceProviderCaching();
            options.EnableDetailedErrors(false); // Disable in production for security
            
            // Configure connection pool settings
            options.ConfigureWarnings(warnings =>
                warnings.Log(RelationalEventId.CommandExecuting));
        }, poolSize: 128); // Allow up to 128 pooled contexts
        
        // Repositories
        services.AddScoped<IScanResultRepository, ScanResultRepository>();
        services.AddScoped<IScanRepository, ScanRepository>();
        services.AddScoped<IScanConfigurationRepository, ScanConfigurationRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        
        // Database initialization
        services.AddScoped<DatabaseInitializer>();
        
        // HTTP
        services.AddHttpClient<SecureHttpClient>()
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("User-Agent", "VaultScope/1.0");
            });
        
        services.AddScoped<IHttpClientService>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<SecureHttpClient>>();
            return new SecureHttpClient(logger, new SecureHttpClientOptions
            {
                TimeoutMs = 30000,
                AllowInsecureLocalhost = true
            });
        });
        
        services.AddScoped<HttpRequestBuilder>();
        services.AddScoped<ResponseAnalyzer>();
        
        // Report Generators
        services.AddScoped<IReportGenerator, HtmlReportGenerator>();
        services.AddScoped<IReportGenerator, JsonReportGenerator>();
        services.AddScoped<IReportGenerator, PdfReportGenerator>();
        
        // Security Services
        services.AddScoped<IUrlValidator, LocalhostValidator>();
        
        // Vulnerability Detectors
        services.AddScoped<IVulnerabilityDetector, SqlInjectionDetector>();
        services.AddScoped<IVulnerabilityDetector, XssDetector>();
        services.AddScoped<IVulnerabilityDetector, XxeDetector>();
        services.AddScoped<IVulnerabilityDetector, CommandInjectionDetector>();
        services.AddScoped<IVulnerabilityDetector, PathTraversalDetector>();
        services.AddScoped<IVulnerabilityDetector, AuthenticationBypassDetector>();
        services.AddScoped<IVulnerabilityDetector, RateLimitingDetector>();
        services.AddScoped<IVulnerabilityDetector, SecurityHeadersDetector>();
        
        // Core Services
        services.AddScoped<ISecurityScanner, SecurityScannerService>();
        services.AddScoped<SecurityScoreCalculator>();
        services.AddScoped<VulnerabilityAnalyzer>();
        
        return services;
    }
    
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        await initializer.InitializeAsync();
    }
}