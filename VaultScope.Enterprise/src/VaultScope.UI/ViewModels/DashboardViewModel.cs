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

public class DashboardViewModel : ViewModelBase
{
    private readonly IScanRepository _scanRepository;
    private readonly ILogger<DashboardViewModel> _logger;
    
    private int _totalScans;
    private int _criticalVulnerabilities;
    private int _highVulnerabilities;
    private int _mediumVulnerabilities;
    private int _lowVulnerabilities;
    private double _averageSecurityScore;
    private string _currentSecurityGrade = "N/A";
    private bool _isLoading = true;

    public DashboardViewModel(IScanRepository scanRepository, ILogger<DashboardViewModel> logger)
    {
        _scanRepository = scanRepository;
        _logger = logger;
        
        RecentScans = new ObservableCollection<ScanResult>();
        SecurityTrends = new ObservableCollection<SecurityTrendItem>();
        
        LoadDataCommand = ReactiveCommand.CreateFromTask(LoadDashboardData);
        RefreshCommand = ReactiveCommand.CreateFromTask(RefreshData);
        
        // Load data on initialization
        LoadDataCommand.Execute().Subscribe();
    }

    public ObservableCollection<ScanResult> RecentScans { get; }
    public ObservableCollection<SecurityTrendItem> SecurityTrends { get; }

    public int TotalScans
    {
        get => _totalScans;
        set => this.RaiseAndSetIfChanged(ref _totalScans, value);
    }

    public int CriticalVulnerabilities
    {
        get => _criticalVulnerabilities;
        set => this.RaiseAndSetIfChanged(ref _criticalVulnerabilities, value);
    }

    public int HighVulnerabilities
    {
        get => _highVulnerabilities;
        set => this.RaiseAndSetIfChanged(ref _highVulnerabilities, value);
    }

    public int MediumVulnerabilities
    {
        get => _mediumVulnerabilities;
        set => this.RaiseAndSetIfChanged(ref _mediumVulnerabilities, value);
    }

    public int LowVulnerabilities
    {
        get => _lowVulnerabilities;
        set => this.RaiseAndSetIfChanged(ref _lowVulnerabilities, value);
    }

    public double AverageSecurityScore
    {
        get => _averageSecurityScore;
        set => this.RaiseAndSetIfChanged(ref _averageSecurityScore, value);
    }

    public string CurrentSecurityGrade
    {
        get => _currentSecurityGrade;
        set => this.RaiseAndSetIfChanged(ref _currentSecurityGrade, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }
    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

    private async Task LoadDashboardData()
    {
        try
        {
            IsLoading = true;
            _logger.LogInformation("Loading dashboard data");

            // Load recent scans
            var recentScans = await _scanRepository.GetRecentScanResultsAsync(10);
            
            RecentScans.Clear();
            foreach (var scan in recentScans)
            {
                RecentScans.Add(scan);
            }

            // Calculate metrics
            var allScans = await _scanRepository.GetAllScanResultsAsync();
            TotalScans = allScans.Count();

            var allVulnerabilities = allScans.SelectMany(s => s.Vulnerabilities).ToList();
            CriticalVulnerabilities = allVulnerabilities.Count(v => v.Severity == VulnerabilitySeverity.Critical);
            HighVulnerabilities = allVulnerabilities.Count(v => v.Severity == VulnerabilitySeverity.High);
            MediumVulnerabilities = allVulnerabilities.Count(v => v.Severity == VulnerabilitySeverity.Medium);
            LowVulnerabilities = allVulnerabilities.Count(v => v.Severity == VulnerabilitySeverity.Low);

            // Calculate average security score
            var completedScans = allScans.Where(s => s.Status == ScanStatus.Completed && s.SecurityScore != null).ToList();
            if (completedScans.Any())
            {
                AverageSecurityScore = completedScans.Average(s => s.SecurityScore.OverallScore);
                CurrentSecurityGrade = SecurityScore.CalculateGrade(AverageSecurityScore);
            }
            else
            {
                AverageSecurityScore = 0;
                CurrentSecurityGrade = "N/A";
            }

            // Generate security trends (last 30 days)
            GenerateSecurityTrends(completedScans);

            _logger.LogInformation("Dashboard data loaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load dashboard data");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void GenerateSecurityTrends(IEnumerable<ScanResult> scans)
    {
        SecurityTrends.Clear();
        
        var last30Days = DateTime.UtcNow.AddDays(-30);
        var trendsData = scans
            .Where(s => s.StartTime >= last30Days)
            .GroupBy(s => s.StartTime.Date)
            .Select(g => new SecurityTrendItem
            {
                Date = g.Key,
                AverageScore = g.Average(s => s.SecurityScore.OverallScore),
                ScanCount = g.Count(),
                VulnerabilityCount = g.Sum(s => s.Vulnerabilities.Count)
            })
            .OrderBy(t => t.Date);

        foreach (var trend in trendsData)
        {
            SecurityTrends.Add(trend);
        }
    }

    private async Task RefreshData()
    {
        await LoadDashboardData();
    }
}

public class SecurityTrendItem
{
    public DateTime Date { get; set; }
    public double AverageScore { get; set; }
    public int ScanCount { get; set; }
    public int VulnerabilityCount { get; set; }
}