using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using VaultScope.Core.Interfaces;
using VaultScope.Core.Models;

namespace VaultScope.UI.ViewModels;

public class ScanConfigurationViewModel : ViewModelBase
{
    private readonly ISecurityScanner _securityScanner;
    private readonly ILogger<ScanConfigurationViewModel> _logger;
    
    private string _targetUrl = "http://localhost:3000";
    private ScanDepth _selectedDepth = ScanDepth.Normal;
    private int _maxRequestsPerSecond = 10;
    private int _maxConcurrentRequests = 5;
    private int _requestTimeout = 30000;
    private bool _followRedirects = true;
    private int _maxRedirects = 5;
    private bool _testAllHttpMethods = true;
    private bool _generateReport = true;
    private bool _isScanning = false;
    private string _scanStatus = "Ready";

    public ScanConfigurationViewModel(ISecurityScanner securityScanner, ILogger<ScanConfigurationViewModel> logger)
    {
        _securityScanner = securityScanner;
        _logger = logger;
        
        SelectedVulnerabilityTypes = new ObservableCollection<VulnerabilityTypeItem>();
        SelectedReportFormats = new ObservableCollection<ReportFormatItem>();
        IncludedPaths = new ObservableCollection<string> { "/" };
        ExcludedPaths = new ObservableCollection<string>();
        
        InitializeVulnerabilityTypes();
        InitializeReportFormats();
        
        StartScanCommand = ReactiveCommand.CreateFromTask(StartScan, this.WhenAnyValue(x => x.IsScanning, scanning => !scanning));
        StartQuickScanCommand = ReactiveCommand.CreateFromTask(StartQuickScan, this.WhenAnyValue(x => x.IsScanning, scanning => !scanning));
        SaveConfigurationCommand = ReactiveCommand.CreateFromTask(SaveConfiguration);
        LoadConfigurationCommand = ReactiveCommand.CreateFromTask(LoadConfiguration);
        ValidateUrlCommand = ReactiveCommand.Create(ValidateUrl);
    }

    public string TargetUrl
    {
        get => _targetUrl;
        set => this.RaiseAndSetIfChanged(ref _targetUrl, value);
    }

    public ScanDepth SelectedDepth
    {
        get => _selectedDepth;
        set => this.RaiseAndSetIfChanged(ref _selectedDepth, value);
    }

    public int MaxRequestsPerSecond
    {
        get => _maxRequestsPerSecond;
        set => this.RaiseAndSetIfChanged(ref _maxRequestsPerSecond, value);
    }

    public int MaxConcurrentRequests
    {
        get => _maxConcurrentRequests;
        set => this.RaiseAndSetIfChanged(ref _maxConcurrentRequests, value);
    }

    public int RequestTimeout
    {
        get => _requestTimeout;
        set => this.RaiseAndSetIfChanged(ref _requestTimeout, value);
    }

    public bool FollowRedirects
    {
        get => _followRedirects;
        set => this.RaiseAndSetIfChanged(ref _followRedirects, value);
    }

    public int MaxRedirects
    {
        get => _maxRedirects;
        set => this.RaiseAndSetIfChanged(ref _maxRedirects, value);
    }

    public bool TestAllHttpMethods
    {
        get => _testAllHttpMethods;
        set => this.RaiseAndSetIfChanged(ref _testAllHttpMethods, value);
    }

    public bool GenerateReport
    {
        get => _generateReport;
        set => this.RaiseAndSetIfChanged(ref _generateReport, value);
    }

    public bool IsScanning
    {
        get => _isScanning;
        set => this.RaiseAndSetIfChanged(ref _isScanning, value);
    }

    public string ScanStatus
    {
        get => _scanStatus;
        set => this.RaiseAndSetIfChanged(ref _scanStatus, value);
    }

    public ObservableCollection<VulnerabilityTypeItem> SelectedVulnerabilityTypes { get; }
    public ObservableCollection<ReportFormatItem> SelectedReportFormats { get; }
    public ObservableCollection<string> IncludedPaths { get; }
    public ObservableCollection<string> ExcludedPaths { get; }

    public ReactiveCommand<Unit, Unit> StartScanCommand { get; }
    public ReactiveCommand<Unit, Unit> StartQuickScanCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveConfigurationCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadConfigurationCommand { get; }
    public ReactiveCommand<Unit, Unit> ValidateUrlCommand { get; }

    private void InitializeVulnerabilityTypes()
    {
        var vulnerabilityTypes = Enum.GetValues<VulnerabilityType>();
        foreach (var type in vulnerabilityTypes)
        {
            SelectedVulnerabilityTypes.Add(new VulnerabilityTypeItem
            {
                Type = type,
                IsSelected = true,
                DisplayName = GetVulnerabilityTypeDisplayName(type)
            });
        }
    }

    private void InitializeReportFormats()
    {
        var reportFormats = Enum.GetValues<ReportFormat>();
        foreach (var format in reportFormats)
        {
            SelectedReportFormats.Add(new ReportFormatItem
            {
                Format = format,
                IsSelected = format == ReportFormat.Html || format == ReportFormat.Json,
                DisplayName = format.ToString()
            });
        }
    }

    private string GetVulnerabilityTypeDisplayName(VulnerabilityType type)
    {
        return type switch
        {
            VulnerabilityType.SqlInjection => "SQL Injection",
            VulnerabilityType.CrossSiteScripting => "Cross-Site Scripting (XSS)",
            VulnerabilityType.XmlExternalEntity => "XML External Entity (XXE)",
            VulnerabilityType.CommandInjection => "Command Injection",
            VulnerabilityType.PathTraversal => "Path Traversal",
            VulnerabilityType.AuthenticationBypass => "Authentication Bypass",
            VulnerabilityType.BrokenAccessControl => "Broken Access Control",
            VulnerabilityType.SecurityMisconfiguration => "Security Misconfiguration",
            VulnerabilityType.SensitiveDataExposure => "Sensitive Data Exposure",
            VulnerabilityType.MissingSecurityHeaders => "Missing Security Headers",
            VulnerabilityType.InsecureDeserialization => "Insecure Deserialization",
            VulnerabilityType.ServerSideRequestForgery => "Server-Side Request Forgery (SSRF)",
            VulnerabilityType.RateLimiting => "Rate Limiting Issues",
            _ => type.ToString()
        };
    }

    private async Task StartScan()
    {
        try
        {
            IsScanning = true;
            ScanStatus = "Starting scan...";
            
            var configuration = CreateScanConfiguration();
            
            _logger.LogInformation("Starting security scan for {Url}", configuration.TargetUrl);
            
            var result = await _securityScanner.ScanAsync(configuration);
            
            ScanStatus = $"Scan completed. Found {result.Vulnerabilities.Count} vulnerabilities.";
            _logger.LogInformation("Scan completed successfully");
            
        }
        catch (Exception ex)
        {
            ScanStatus = $"Scan failed: {ex.Message}";
            _logger.LogError(ex, "Scan failed");
        }
        finally
        {
            IsScanning = false;
        }
    }

    public async Task StartQuickScan()
    {
        try
        {
            IsScanning = true;
            ScanStatus = "Starting quick scan...";
            
            _logger.LogInformation("Starting quick scan for {Url}", TargetUrl);
            
            var result = await _securityScanner.QuickScanAsync(TargetUrl);
            
            ScanStatus = $"Quick scan completed. Found {result.Vulnerabilities.Count} vulnerabilities.";
            _logger.LogInformation("Quick scan completed successfully");
            
        }
        catch (Exception ex)
        {
            ScanStatus = $"Quick scan failed: {ex.Message}";
            _logger.LogError(ex, "Quick scan failed");
        }
        finally
        {
            IsScanning = false;
        }
    }

    private ScanConfiguration CreateScanConfiguration()
    {
        return new ScanConfiguration
        {
            TargetUrl = TargetUrl,
            Depth = SelectedDepth,
            MaxRequestsPerSecond = MaxRequestsPerSecond,
            MaxConcurrentRequests = MaxConcurrentRequests,
            RequestTimeout = RequestTimeout,
            FollowRedirects = FollowRedirects,
            MaxRedirects = MaxRedirects,
            TestAllHttpMethods = TestAllHttpMethods,
            GenerateReport = GenerateReport,
            VulnerabilityTypes = SelectedVulnerabilityTypes
                .Where(v => v.IsSelected)
                .Select(v => v.Type)
                .ToList(),
            ReportFormats = SelectedReportFormats
                .Where(r => r.IsSelected)
                .Select(r => r.Format)
                .ToList(),
            IncludedPaths = IncludedPaths.ToList(),
            ExcludedPaths = ExcludedPaths.ToList()
        };
    }

    private async Task SaveConfiguration()
    {
        // Implementation for saving configuration
        await Task.CompletedTask;
    }

    private async Task LoadConfiguration()
    {
        // Implementation for loading configuration
        await Task.CompletedTask;
    }

    private void ValidateUrl()
    {
        // Implementation for URL validation
        if (string.IsNullOrWhiteSpace(TargetUrl))
        {
            ScanStatus = "Please enter a target URL";
            return;
        }

        if (!Uri.TryCreate(TargetUrl, UriKind.Absolute, out var uri))
        {
            ScanStatus = "Invalid URL format";
            return;
        }

        if (uri.Scheme != "http" && uri.Scheme != "https")
        {
            ScanStatus = "URL must use HTTP or HTTPS protocol";
            return;
        }

        if (!uri.Host.Contains("localhost") && !uri.Host.StartsWith("127.0.0.1"))
        {
            ScanStatus = "Only localhost URLs are allowed for security";
            return;
        }

        ScanStatus = "URL is valid";
    }
}

public class VulnerabilityTypeItem
{
    public VulnerabilityType Type { get; set; }
    public bool IsSelected { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}

public class ReportFormatItem
{
    public ReportFormat Format { get; set; }
    public bool IsSelected { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}