using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using VaultScope.Core.Interfaces;
using VaultScope.Core.Models;
using Models = VaultScope.Core.Models;

namespace VaultScope.UI.ViewModels;

public class ScanProgressViewModel : ViewModelBase
{
    private readonly ISecurityScanner _securityScanner;
    private readonly ILogger<ScanProgressViewModel> _logger;
    private CancellationTokenSource? _cancellationTokenSource;
    
    private double _progressPercentage;
    private string _currentTask = "Initializing...";
    private int _vulnerabilitiesFound;
    private int _endpointsTested;
    private TimeSpan _elapsedTime;
    private string _scanStatus = "Ready";
    private bool _isScanning;
    private int _totalRequests;
    private int _completedRequests;
    private double _requestsPerSecond;
    private ScanResult? _currentScanResult;

    public ScanProgressViewModel(ISecurityScanner securityScanner, ILogger<ScanProgressViewModel> logger)
    {
        _securityScanner = securityScanner;
        _logger = logger;
        
        ActiveDetectors = new ObservableCollection<DetectorStatus>();
        RecentVulnerabilities = new ObservableCollection<Vulnerability>();
        
        CancelScanCommand = ReactiveCommand.Create(CancelScan, this.WhenAnyValue(x => x.IsScanning));
        PauseScanCommand = ReactiveCommand.Create(PauseScan, this.WhenAnyValue(x => x.IsScanning));
        ResumeScanCommand = ReactiveCommand.Create(ResumeScan, this.WhenAnyValue(x => x.IsScanning, scanning => !scanning));
        
        // Subscribe to scanner events
        _securityScanner.ProgressChanged += OnProgressChanged;
        _securityScanner.VulnerabilityDetected += OnVulnerabilityDetected;
    }

    public double ProgressPercentage
    {
        get => _progressPercentage;
        set => this.RaiseAndSetIfChanged(ref _progressPercentage, value);
    }

    public string CurrentTask
    {
        get => _currentTask;
        set => this.RaiseAndSetIfChanged(ref _currentTask, value);
    }

    public int VulnerabilitiesFound
    {
        get => _vulnerabilitiesFound;
        set => this.RaiseAndSetIfChanged(ref _vulnerabilitiesFound, value);
    }

    public int EndpointsTested
    {
        get => _endpointsTested;
        set => this.RaiseAndSetIfChanged(ref _endpointsTested, value);
    }

    public TimeSpan ElapsedTime
    {
        get => _elapsedTime;
        set => this.RaiseAndSetIfChanged(ref _elapsedTime, value);
    }

    public string ScanStatus
    {
        get => _scanStatus;
        set => this.RaiseAndSetIfChanged(ref _scanStatus, value);
    }

    public bool IsScanning
    {
        get => _isScanning;
        set => this.RaiseAndSetIfChanged(ref _isScanning, value);
    }

    public int TotalRequests
    {
        get => _totalRequests;
        set => this.RaiseAndSetIfChanged(ref _totalRequests, value);
    }

    public int CompletedRequests
    {
        get => _completedRequests;
        set => this.RaiseAndSetIfChanged(ref _completedRequests, value);
    }

    public double RequestsPerSecond
    {
        get => _requestsPerSecond;
        set => this.RaiseAndSetIfChanged(ref _requestsPerSecond, value);
    }

    public ScanResult? CurrentScanResult
    {
        get => _currentScanResult;
        set => this.RaiseAndSetIfChanged(ref _currentScanResult, value);
    }

    public ObservableCollection<DetectorStatus> ActiveDetectors { get; }
    public ObservableCollection<Vulnerability> RecentVulnerabilities { get; }

    public ReactiveCommand<Unit, Unit> CancelScanCommand { get; }
    public ReactiveCommand<Unit, Unit> PauseScanCommand { get; }
    public ReactiveCommand<Unit, Unit> ResumeScanCommand { get; }

    public async Task StartScan(ScanConfiguration configuration)
    {
        try
        {
            IsScanning = true;
            ScanStatus = "Starting scan...";
            _cancellationTokenSource = new CancellationTokenSource();
            
            // Initialize detector statuses
            InitializeDetectorStatuses(configuration);
            
            // Clear previous results
            RecentVulnerabilities.Clear();
            
            // Reset counters
            ProgressPercentage = 0;
            VulnerabilitiesFound = 0;
            EndpointsTested = 0;
            CompletedRequests = 0;
            
            _logger.LogInformation("Starting scan for {Url}", configuration.TargetUrl);
            
            var result = await _securityScanner.ScanAsync(configuration, _cancellationTokenSource.Token);
            
            CurrentScanResult = result;
            ScanStatus = result.Status switch
            {
                Models.ScanStatus.Completed => "Scan completed successfully",
                Models.ScanStatus.Cancelled => "Scan was cancelled",
                Models.ScanStatus.Failed => $"Scan failed: {result.ErrorMessage}",
                _ => "Scan finished"
            };
            
            _logger.LogInformation("Scan completed with status: {Status}", result.Status);
        }
        catch (OperationCanceledException)
        {
            ScanStatus = "Scan was cancelled";
            _logger.LogWarning("Scan was cancelled by user");
        }
        catch (Exception ex)
        {
            ScanStatus = $"Scan failed: {ex.Message}";
            _logger.LogError(ex, "Scan failed with error");
        }
        finally
        {
            IsScanning = false;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    private void InitializeDetectorStatuses(ScanConfiguration configuration)
    {
        ActiveDetectors.Clear();
        
        foreach (var vulnerabilityType in configuration.VulnerabilityTypes)
        {
            ActiveDetectors.Add(new DetectorStatus
            {
                Name = GetDetectorName(vulnerabilityType),
                Type = vulnerabilityType,
                Status = DetectorStatusType.Pending,
                Progress = 0,
                VulnerabilitiesFound = 0,
                LastActivity = DateTime.UtcNow
            });
        }
    }

    private string GetDetectorName(VulnerabilityType type)
    {
        return type switch
        {
            VulnerabilityType.SqlInjection => "SQL Injection Detector",
            VulnerabilityType.CrossSiteScripting => "XSS Detector",
            VulnerabilityType.XmlExternalEntity => "XXE Detector",
            VulnerabilityType.CommandInjection => "Command Injection Detector",
            VulnerabilityType.PathTraversal => "Path Traversal Detector",
            VulnerabilityType.AuthenticationBypass => "Authentication Bypass Detector",
            VulnerabilityType.BrokenAccessControl => "Access Control Detector",
            VulnerabilityType.SecurityMisconfiguration => "Security Config Detector",
            VulnerabilityType.SensitiveDataExposure => "Data Exposure Detector",
            VulnerabilityType.MissingSecurityHeaders => "Security Headers Detector",
            VulnerabilityType.InsecureDeserialization => "Deserialization Detector",
            VulnerabilityType.ServerSideRequestForgery => "SSRF Detector",
            VulnerabilityType.RateLimiting => "Rate Limiting Detector",
            _ => $"{type} Detector"
        };
    }

    private void OnProgressChanged(object? sender, ScanProgressEventArgs e)
    {
        ProgressPercentage = e.ProgressPercentage;
        CurrentTask = e.CurrentTask;
        VulnerabilitiesFound = e.VulnerabilitiesFound;
        EndpointsTested = e.EndpointsTested;
        ElapsedTime = e.ElapsedTime;
        
        // Calculate requests per second
        if (e.ElapsedTime.TotalSeconds > 0)
        {
            RequestsPerSecond = CompletedRequests / e.ElapsedTime.TotalSeconds;
        }
        
        ScanStatus = $"Scanning... {e.ProgressPercentage:F1}% complete";
    }

    private void OnVulnerabilityDetected(object? sender, VulnerabilityDetectedEventArgs e)
    {
        // Add to recent vulnerabilities (keep only last 10)
        RecentVulnerabilities.Insert(0, e.Vulnerability);
        while (RecentVulnerabilities.Count > 10)
        {
            RecentVulnerabilities.RemoveAt(RecentVulnerabilities.Count - 1);
        }
        
        // Update detector status
        var detectorStatus = ActiveDetectors.FirstOrDefault(d => 
            GetVulnerabilityTypeFromDetector(d.Name) == GetVulnerabilityTypeFromString(e.Vulnerability.Type));
        
        if (detectorStatus != null)
        {
            detectorStatus.VulnerabilitiesFound++;
            detectorStatus.LastActivity = DateTime.UtcNow;
            detectorStatus.Status = DetectorStatusType.Active;
        }
        
        VulnerabilitiesFound++;
    }

    private VulnerabilityType GetVulnerabilityTypeFromDetector(string detectorName)
    {
        return detectorName switch
        {
            "SQL Injection Detector" => VulnerabilityType.SqlInjection,
            "XSS Detector" => VulnerabilityType.CrossSiteScripting,
            "XXE Detector" => VulnerabilityType.XmlExternalEntity,
            "Command Injection Detector" => VulnerabilityType.CommandInjection,
            "Path Traversal Detector" => VulnerabilityType.PathTraversal,
            "Authentication Bypass Detector" => VulnerabilityType.AuthenticationBypass,
            "Access Control Detector" => VulnerabilityType.BrokenAccessControl,
            "Security Config Detector" => VulnerabilityType.SecurityMisconfiguration,
            "Data Exposure Detector" => VulnerabilityType.SensitiveDataExposure,
            "Security Headers Detector" => VulnerabilityType.MissingSecurityHeaders,
            "Deserialization Detector" => VulnerabilityType.InsecureDeserialization,
            "SSRF Detector" => VulnerabilityType.ServerSideRequestForgery,
            "Rate Limiting Detector" => VulnerabilityType.RateLimiting,
            _ => VulnerabilityType.SqlInjection
        };
    }

    private VulnerabilityType GetVulnerabilityTypeFromString(string type)
    {
        return Enum.TryParse<VulnerabilityType>(type, out var result) ? result : VulnerabilityType.SqlInjection;
    }

    private void CancelScan()
    {
        _cancellationTokenSource?.Cancel();
        ScanStatus = "Cancelling scan...";
        _logger.LogInformation("User cancelled scan");
    }

    private void PauseScan()
    {
        // Implementation for pausing scan
        ScanStatus = "Scan paused";
    }

    private void ResumeScan()
    {
        // Implementation for resuming scan
        ScanStatus = "Resuming scan...";
    }
}

public class DetectorStatus
{
    public string Name { get; set; } = string.Empty;
    public VulnerabilityType Type { get; set; }
    public DetectorStatusType Status { get; set; }
    public double Progress { get; set; }
    public int VulnerabilitiesFound { get; set; }
    public DateTime LastActivity { get; set; }
}

public enum DetectorStatusType
{
    Pending,
    Active,
    Completed,
    Failed
}

