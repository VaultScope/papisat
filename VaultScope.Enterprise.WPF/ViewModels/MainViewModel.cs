using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace VaultScope.Enterprise.WPF.ViewModels;

/// <summary>
/// Main view model that handles navigation state and application-wide state
/// </summary>
public partial class MainViewModel : BaseViewModel
{
    private readonly IMessenger _messenger;

    public MainViewModel(IMessenger messenger)
    {
        _messenger = messenger;
        CurrentPage = "SecurityDashboard";
    }

    /// <summary>
    /// The currently active page
    /// </summary>
    [ObservableProperty]
    private string currentPage = "SecurityDashboard";

    /// <summary>
    /// Navigation command for Security Dashboard
    /// </summary>
    [RelayCommand]
    private void NavigateToSecurityDashboard()
    {
        CurrentPage = "SecurityDashboard";
        _messenger.Send(new NavigationMessage("SecurityDashboard"));
    }

    /// <summary>
    /// Navigation command for Vulnerability Scanner
    /// </summary>
    [RelayCommand]
    private void NavigateToVulnerabilityScanner()
    {
        CurrentPage = "VulnerabilityScanner";
        _messenger.Send(new NavigationMessage("VulnerabilityScanner"));
    }

    /// <summary>
    /// Navigation command for Reports & Analytics
    /// </summary>
    [RelayCommand]
    private void NavigateToReports()
    {
        CurrentPage = "Reports";
        _messenger.Send(new NavigationMessage("Reports"));
    }

    /// <summary>
    /// Navigation command for Settings
    /// </summary>
    [RelayCommand]
    private void NavigateToSettings()
    {
        CurrentPage = "Settings";
        _messenger.Send(new NavigationMessage("Settings"));
    }

    /// <summary>
    /// Quick action command for security scan
    /// </summary>
    [RelayCommand]
    private void QuickSecurityScan()
    {
        CurrentPage = "VulnerabilityScanner";
        _messenger.Send(new NavigationMessage("VulnerabilityScanner"));
        _messenger.Send(new QuickScanMessage());
    }

    /// <summary>
    /// Quick action command for initializing security scan
    /// </summary>
    [RelayCommand]
    private void InitializeSecurityScan()
    {
        CurrentPage = "VulnerabilityScanner";
        _messenger.Send(new NavigationMessage("VulnerabilityScanner"));
        _messenger.Send(new InitializeScanMessage());
    }
}

/// <summary>
/// Navigation message for communicating page changes
/// </summary>
public record NavigationMessage(string PageName);

/// <summary>
/// Quick scan message for triggering quick scan actions
/// </summary>
public record QuickScanMessage();

/// <summary>
/// Initialize scan message for triggering scan initialization
/// </summary>
public record InitializeScanMessage();