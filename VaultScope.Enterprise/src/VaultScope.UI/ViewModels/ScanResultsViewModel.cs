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

public class ScanResultsViewModel : ViewModelBase
{
    private readonly IScanRepository _scanRepository;
    private readonly IReportGenerator _reportGenerator;
    private readonly ILogger<ScanResultsViewModel> _logger;
    
    private ScanResult? _selectedScanResult;
    private string _searchText = string.Empty;
    private VulnerabilitySeverity? _selectedSeverityFilter;
    private string _selectedStatusFilter = "All";
    private bool _isLoading;
    private int _totalScans;
    private int _totalVulnerabilities;
    private double _averageScore;

    public ScanResultsViewModel(
        IScanRepository scanRepository, 
        IReportGenerator reportGenerator,
        ILogger<ScanResultsViewModel> logger)
    {
        _scanRepository = scanRepository;
        _reportGenerator = reportGenerator;
        _logger = logger;
        
        ScanResults = new ObservableCollection<ScanResult>();
        FilteredVulnerabilities = new ObservableCollection<Vulnerability>();
        
        LoadResultsCommand = ReactiveCommand.CreateFromTask(LoadResults);
        RefreshCommand = ReactiveCommand.CreateFromTask(RefreshResults);
        ExportResultsCommand = ReactiveCommand.CreateFromTask<ReportFormat>(ExportResults);
        DeleteScanCommand = ReactiveCommand.CreateFromTask<ScanResult>(DeleteScan);
        ViewDetailsCommand = ReactiveCommand.Create<ScanResult>(ViewDetails);
        
        // Load results on initialization
        LoadResultsCommand.Execute().Subscribe();
        
        // Set up filtering
        this.WhenAnyValue(x => x.SelectedScanResult)
            .Subscribe(result => FilterVulnerabilities(result));
            
        this.WhenAnyValue(x => x.SearchText, x => x.SelectedSeverityFilter)
            .Subscribe(_ => FilterVulnerabilities(SelectedScanResult));
    }

    public ObservableCollection<ScanResult> ScanResults { get; }
    public ObservableCollection<Vulnerability> FilteredVulnerabilities { get; }

    public ScanResult? SelectedScanResult
    {
        get => _selectedScanResult;
        set => this.RaiseAndSetIfChanged(ref _selectedScanResult, value);
    }

    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    public VulnerabilitySeverity? SelectedSeverityFilter
    {
        get => _selectedSeverityFilter;
        set => this.RaiseAndSetIfChanged(ref _selectedSeverityFilter, value);
    }

    public string SelectedStatusFilter
    {
        get => _selectedStatusFilter;
        set => this.RaiseAndSetIfChanged(ref _selectedStatusFilter, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public int TotalScans
    {
        get => _totalScans;
        set => this.RaiseAndSetIfChanged(ref _totalScans, value);
    }

    public int TotalVulnerabilities
    {
        get => _totalVulnerabilities;
        set => this.RaiseAndSetIfChanged(ref _totalVulnerabilities, value);
    }

    public double AverageScore
    {
        get => _averageScore;
        set => this.RaiseAndSetIfChanged(ref _averageScore, value);
    }

    public ReactiveCommand<Unit, Unit> LoadResultsCommand { get; }
    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
    public ReactiveCommand<ReportFormat, Unit> ExportResultsCommand { get; }
    public ReactiveCommand<ScanResult, Unit> DeleteScanCommand { get; }
    public ReactiveCommand<ScanResult, Unit> ViewDetailsCommand { get; }

    private async Task LoadResults()
    {
        try
        {
            IsLoading = true;
            _logger.LogInformation("Loading scan results");

            var results = await _scanRepository.GetAllScanResultsAsync();
            
            ScanResults.Clear();
            foreach (var result in results.OrderByDescending(r => r.StartTime))
            {
                ScanResults.Add(result);
            }

            // Calculate summary statistics
            TotalScans = results.Count();
            TotalVulnerabilities = results.SelectMany(r => r.Vulnerabilities).Count();
            
            var completedScans = results.Where(r => r.Status == ScanStatus.Completed).ToList();
            if (completedScans.Any())
            {
                AverageScore = completedScans.Average(r => r.SecurityScore?.OverallScore ?? 0);
            }

            _logger.LogInformation("Loaded {Count} scan results", ScanResults.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load scan results");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task RefreshResults()
    {
        await LoadResults();
    }

    private async Task ExportResults(ReportFormat format)
    {
        try
        {
            if (SelectedScanResult == null)
            {
                _logger.LogWarning("No scan result selected for export");
                return;
            }

            _logger.LogInformation("Exporting scan result {Id} to {Format}", 
                SelectedScanResult.Id, format);

            var reportData = await _reportGenerator.GenerateAsync(SelectedScanResult, new ReportOptions());
            
            // In a real implementation, this would save the file or open a save dialog
            _logger.LogInformation("Report generated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export results");
        }
    }

    private async Task DeleteScan(ScanResult scanResult)
    {
        try
        {
            _logger.LogInformation("Deleting scan result {Id}", scanResult.Id);
            
            await _scanRepository.DeleteScanResultAsync(scanResult.Id);
            ScanResults.Remove(scanResult);
            
            if (SelectedScanResult == scanResult)
            {
                SelectedScanResult = null;
            }
            
            _logger.LogInformation("Scan result deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete scan result");
        }
    }

    private void ViewDetails(ScanResult scanResult)
    {
        SelectedScanResult = scanResult;
        _logger.LogInformation("Viewing details for scan {Id}", scanResult.Id);
    }

    private void FilterVulnerabilities(ScanResult? scanResult)
    {
        FilteredVulnerabilities.Clear();
        
        if (scanResult == null)
            return;

        var vulnerabilities = scanResult.Vulnerabilities.AsEnumerable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            vulnerabilities = vulnerabilities.Where(v => 
                v.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                v.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                v.AffectedEndpoint.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        // Apply severity filter
        if (SelectedSeverityFilter.HasValue)
        {
            vulnerabilities = vulnerabilities.Where(v => v.Severity == SelectedSeverityFilter.Value);
        }

        // Add filtered vulnerabilities
        foreach (var vulnerability in vulnerabilities.OrderByDescending(v => v.Severity))
        {
            FilteredVulnerabilities.Add(vulnerability);
        }
    }

    public string GetSeverityDisplayName(VulnerabilitySeverity severity)
    {
        return severity switch
        {
            VulnerabilitySeverity.Critical => "Critical",
            VulnerabilitySeverity.High => "High",
            VulnerabilitySeverity.Medium => "Medium",
            VulnerabilitySeverity.Low => "Low",
            VulnerabilitySeverity.Informational => "Informational",
            _ => severity.ToString()
        };
    }

    public string GetStatusDisplayName(ScanStatus status)
    {
        return status switch
        {
            ScanStatus.Pending => "Pending",
            ScanStatus.Testing => "Running",
            ScanStatus.Completed => "Completed",
            ScanStatus.Failed => "Failed",
            ScanStatus.Cancelled => "Cancelled",
            _ => status.ToString()
        };
    }

    public string GetStatusColor(ScanStatus status)
    {
        return status switch
        {
            ScanStatus.Pending => "WarningBrush",
            ScanStatus.Testing => "InfoBrush",
            ScanStatus.Completed => "SuccessBrush",
            ScanStatus.Failed => "ErrorBrush",
            ScanStatus.Cancelled => "TextMutedBrush",
            _ => "TextSecondaryBrush"
        };
    }

    public string GetSeverityColor(VulnerabilitySeverity severity)
    {
        return severity switch
        {
            VulnerabilitySeverity.Critical => "CriticalBrush",
            VulnerabilitySeverity.High => "HighBrush",
            VulnerabilitySeverity.Medium => "MediumBrush",
            VulnerabilitySeverity.Low => "LowBrush",
            VulnerabilitySeverity.Informational => "InfoSecondaryBrush",
            _ => "TextSecondaryBrush"
        };
    }
}