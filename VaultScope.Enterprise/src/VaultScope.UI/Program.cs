using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Avalonia.Win32;
using Avalonia.X11;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VaultScope.Infrastructure;
using VaultScope.UI.ViewModels;

namespace VaultScope.UI;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Application startup failed: {ex}");
            throw;
        }
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .With(new Win32PlatformOptions
            {
                RenderingMode = new[] { Win32RenderingMode.Software }
            })
            .With(new X11PlatformOptions
            {
                RenderingMode = new[] { X11RenderingMode.Software }
            })
            .LogToTrace()
            .UseReactiveUI()
            .WithInterFont();
    }

    public static IHost CreateHost(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddInfrastructure(context.Configuration);
                services.AddSingleton<MainWindowViewModel>();
                services.AddTransient<DashboardViewModel>();
                services.AddTransient<ScanConfigurationViewModel>();
                services.AddTransient<ScanProgressViewModel>();
                services.AddTransient<ScanResultsViewModel>();
                services.AddTransient<VulnerabilityDetailsViewModel>();
                services.AddTransient<SettingsViewModel>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .Build();
    }
}