using System;
using System.Globalization;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using VaultScope.Core.Interfaces;
using Avalonia.Collections;
using System.Collections.Generic;

namespace VaultScope.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISecurityScanner _securityScanner;
    
    private ViewModelBase? _currentViewModel;
    private string _currentView = "Dashboard";
    private bool _isConnected = true;

    public MainWindowViewModel(IServiceProvider serviceProvider, ISecurityScanner securityScanner)
    {
        _serviceProvider = serviceProvider;
        _securityScanner = securityScanner;
        
        NavigateToCommand = ReactiveCommand.Create<string>(NavigateTo);
        StartQuickScanCommand = ReactiveCommand.CreateFromTask(StartQuickScan);
        
        // Initialize with Dashboard
        NavigateTo("Dashboard");
    }

    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        private set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    public string CurrentView
    {
        get => _currentView;
        private set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

    public bool IsConnected
    {
        get => _isConnected;
        set => this.RaiseAndSetIfChanged(ref _isConnected, value);
    }

    public ReactiveCommand<string, Unit> NavigateToCommand { get; }
    public ReactiveCommand<Unit, Unit> StartQuickScanCommand { get; }
    
    // Navigation state properties
    public bool IsDashboardActive => CurrentView == "Dashboard";
    public bool IsScanConfigurationActive => CurrentView == "ScanConfiguration";
    public bool IsScanProgressActive => CurrentView == "ScanProgress";
    public bool IsScanResultsActive => CurrentView == "ScanResults";
    public bool IsVulnerabilityDetailsActive => CurrentView == "VulnerabilityDetails";
    public bool IsSettingsActive => CurrentView == "Settings";

    private void NavigateTo(string viewName)
    {
        CurrentView = viewName;
        
        CurrentViewModel = viewName switch
        {
            "Dashboard" => _serviceProvider.GetRequiredService<DashboardViewModel>(),
            "ScanConfiguration" => _serviceProvider.GetRequiredService<ScanConfigurationViewModel>(),
            "ScanProgress" => _serviceProvider.GetRequiredService<ScanProgressViewModel>(),
            "ScanResults" => _serviceProvider.GetRequiredService<ScanResultsViewModel>(),
            "VulnerabilityDetails" => _serviceProvider.GetRequiredService<VulnerabilityDetailsViewModel>(),
            "Settings" => _serviceProvider.GetRequiredService<SettingsViewModel>(),
            _ => _serviceProvider.GetRequiredService<DashboardViewModel>()
        };
        
        // Trigger property changed for all navigation properties
        this.RaisePropertyChanged(nameof(IsDashboardActive));
        this.RaisePropertyChanged(nameof(IsScanConfigurationActive));
        this.RaisePropertyChanged(nameof(IsScanProgressActive));
        this.RaisePropertyChanged(nameof(IsScanResultsActive));
        this.RaisePropertyChanged(nameof(IsVulnerabilityDetailsActive));
        this.RaisePropertyChanged(nameof(IsSettingsActive));
    }

    private async System.Threading.Tasks.Task StartQuickScan()
    {
        NavigateTo("ScanConfiguration");
        
        if (CurrentViewModel is ScanConfigurationViewModel scanConfigViewModel)
        {
            await scanConfigViewModel.StartQuickScan();
        }
    }

    public static NavigationButtonStyleConverter NavigationButtonStyleConverter { get; } = new();
    public static NavigationButtonClassConverter NavigationButtonClassConverter { get; } = new();
}

public class NavigationButtonStyleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string currentView && parameter is string targetView)
        {
            return currentView == targetView ? "NavigationButtonActive" : "NavigationButton";
        }
        return "NavigationButton";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class NavigationButtonClassConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count >= 2 && values[0] is string currentView && values[1] is string targetView)
        {
            return currentView == targetView ? "NavigationButtonActive" : "NavigationButton";
        }
        
        return "NavigationButton";
    }
}