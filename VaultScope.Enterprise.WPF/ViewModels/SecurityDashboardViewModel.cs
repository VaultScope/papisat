using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace VaultScope.Enterprise.WPF.ViewModels;

/// <summary>
/// View model for the Security Dashboard page
/// </summary>
public partial class SecurityDashboardViewModel : BaseViewModel
{
    public SecurityDashboardViewModel()
    {
        // Initialize with default values as specified
        TotalScans = 0;
        Vulnerabilities = 0;
        SecurityScore = 0.0;
        LastScan = "Never";
        
        // Initialize statistics cards
        StatisticsCards = new ObservableCollection<StatisticsCardViewModel>
        {
            new StatisticsCardViewModel
            {
                Icon = "&#xE9F9;",
                Title = "Total Scans",
                Value = TotalScans.ToString(),
                Description = "security scans",
                Color = "#3b82f6"
            },
            new StatisticsCardViewModel
            {
                Icon = "&#xE7BA;",
                Title = "Vulnerabilities",
                Value = Vulnerabilities.ToString(),
                Description = "critical issues found",
                Color = "#ef4444"
            },
            new StatisticsCardViewModel
            {
                Icon = "&#xE946;",
                Title = "Security Score",
                Value = SecurityScore.ToString("F1"),
                Description = "out of 100 security score",
                Color = "#10b981"
            },
            new StatisticsCardViewModel
            {
                Icon = "&#xE73E;",
                Title = "Last Scan",
                Value = LastScan,
                Description = "no scans completed",
                Color = "#9ca3af"
            }
        };
    }

    /// <summary>
    /// Total number of scans performed
    /// </summary>
    [ObservableProperty]
    private int totalScans;

    /// <summary>
    /// Number of vulnerabilities found
    /// </summary>
    [ObservableProperty]
    private int vulnerabilities;

    /// <summary>
    /// Current security score
    /// </summary>
    [ObservableProperty]
    private double securityScore;

    /// <summary>
    /// Last scan date/time
    /// </summary>
    [ObservableProperty]
    private string lastScan = "Never";

    /// <summary>
    /// Collection of statistics cards for the dashboard
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<StatisticsCardViewModel> statisticsCards;

    /// <summary>
    /// Indicates whether the dashboard is in empty state
    /// </summary>
    public bool IsEmptyState => TotalScans == 0 && Vulnerabilities == 0 && SecurityScore == 0.0;

    /// <summary>
    /// Command to start a new security scan
    /// </summary>
    [RelayCommand]
    private async Task StartSecurityScanAsync()
    {
        IsLoading = true;
        ClearError();
        
        try
        {
            // TODO: Implement actual scan logic
            await Task.Delay(1000); // Simulate scan operation
            
            // Update statistics (this would be replaced with actual scan results)
            TotalScans++;
            LastScan = DateTime.Now.ToString("MMM dd, yyyy HH:mm");
            
            // Update statistics cards
            UpdateStatisticsCards();
            
            OnPropertyChanged(nameof(IsEmptyState));
        }
        catch (Exception ex)
        {
            SetError($"Failed to start security scan: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Command to refresh dashboard data
    /// </summary>
    [RelayCommand]
    private async Task RefreshDashboardAsync()
    {
        IsLoading = true;
        ClearError();
        
        try
        {
            // TODO: Implement actual refresh logic
            await Task.Delay(500); // Simulate refresh operation
            
            // Update statistics cards
            UpdateStatisticsCards();
        }
        catch (Exception ex)
        {
            SetError($"Failed to refresh dashboard: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Updates the statistics cards with current values
    /// </summary>
    private void UpdateStatisticsCards()
    {
        StatisticsCards[0].Value = TotalScans.ToString();
        StatisticsCards[1].Value = Vulnerabilities.ToString();
        StatisticsCards[2].Value = SecurityScore.ToString("F1");
        StatisticsCards[3].Value = LastScan;
        StatisticsCards[3].Description = TotalScans > 0 ? "last scan completed" : "no scans completed";
    }
}

/// <summary>
/// View model for individual statistics cards
/// </summary>
public partial class StatisticsCardViewModel : ObservableObject
{
    [ObservableProperty]
    private string icon = string.Empty;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string value = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string color = string.Empty;
}