using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using VaultScope.Enterprise.WPF.ViewModels;
using VaultScope.Enterprise.WPF.Services;
using VaultScope.Enterprise.WPF.Themes;
using CommunityToolkit.Mvvm.Messaging;

namespace VaultScope.Enterprise.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IHost? _host;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Initialize modern theme system
        ModernThemeManager.Instance.ApplyTheme();
        
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Register Messenger
                services.AddSingleton<IMessenger, WeakReferenceMessenger>();
                
                // Register ViewModels
                services.AddTransient<MainViewModel>();
                services.AddTransient<SecurityDashboardViewModel>();
                services.AddTransient<VulnerabilityScannerViewModel>();
                services.AddTransient<ReportsViewModel>();
                services.AddTransient<SettingsViewModel>();
                
                // Register Services
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<INotificationService, NotificationService>();
                services.AddSingleton<ISecurityScanService, SecurityScanService>();
                services.AddSingleton<IThemeService, ThemeService>();
                
                // Register Theme Managers
                services.AddSingleton<ThemeManager>(ThemeManager.Instance);
                services.AddSingleton<ModernThemeManager>(ModernThemeManager.Instance);
            })
            .Build();

        _host.Start();

        // Create MainWindow manually with the MainViewModel from DI
        var mainViewModel = _host.Services.GetRequiredService<MainViewModel>();
        var mainWindow = new MainWindow(mainViewModel);
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        ModernThemeManager.Instance.Dispose();
        _host?.Dispose();
        base.OnExit(e);
    }
}