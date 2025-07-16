using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VaultScope.UI.ViewModels;
using VaultScope.UI.Views;
using VaultScope.Infrastructure;

namespace VaultScope.UI;

public partial class App : Application
{
    private IHost? _host;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        _host = Program.CreateHost(Environment.GetCommandLineArgs());
        _host.Start();

        // Initialize the database
        _ = Task.Run(async () =>
        {
            try
            {
                await _host.Services.InitializeDatabaseAsync();
            }
            catch (Exception ex)
            {
                // Log the error but don't crash the app
                System.Diagnostics.Debug.WriteLine($"Database initialization failed: {ex.Message}");
            }
        });

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainViewModel = _host.Services.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

}