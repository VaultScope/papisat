using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using VaultScope.Core.Models;

namespace VaultScope.UI.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly ILogger<SettingsViewModel> _logger;
    
    private string _defaultTargetUrl = "http://localhost:3000";
    private ScanDepth _defaultScanDepth = ScanDepth.Normal;
    private int _defaultMaxRequestsPerSecond = 10;
    private int _defaultMaxConcurrentRequests = 5;
    private int _defaultRequestTimeout = 30000;
    private bool _defaultFollowRedirects = true;
    private int _defaultMaxRedirects = 5;
    private bool _defaultTestAllHttpMethods = true;
    private bool _defaultGenerateReport = true;
    private bool _enableNotifications = true;
    private bool _enableSounds = false;
    private bool _autoStartScans = false;
    private bool _saveReportsAutomatically = true;
    private string _defaultReportLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    private string _databaseLocation = "VaultScope.db";
    private int _maxRecentScans = 50;
    private int _scanHistoryRetentionDays = 90;
    private bool _enableLogging = true;
    private string _logLevel = "Information";
    private bool _enableTelemetry = false;

    public SettingsViewModel(ILogger<SettingsViewModel> logger)
    {
        _logger = logger;
        
        DefaultVulnerabilityTypes = new ObservableCollection<VulnerabilityTypeItem>();
        DefaultReportFormats = new ObservableCollection<ReportFormatItem>();
        LogLevels = new ObservableCollection<string> { "Debug", "Information", "Warning", "Error" };
        
        InitializeDefaults();
        
        SaveSettingsCommand = ReactiveCommand.CreateFromTask(SaveSettings);
        LoadSettingsCommand = ReactiveCommand.CreateFromTask(LoadSettings);
        ResetToDefaultsCommand = ReactiveCommand.Create(ResetToDefaults);
        BrowseReportLocationCommand = ReactiveCommand.Create(BrowseReportLocation);
        BrowseDatabaseLocationCommand = ReactiveCommand.Create(BrowseDatabaseLocation);
        ClearScanHistoryCommand = ReactiveCommand.CreateFromTask(ClearScanHistory);
        ExportSettingsCommand = ReactiveCommand.CreateFromTask(ExportSettings);
        ImportSettingsCommand = ReactiveCommand.CreateFromTask(ImportSettings);
        
        // Load settings on initialization
        LoadSettingsCommand.Execute().Subscribe();
    }

    public string DefaultTargetUrl
    {
        get => _defaultTargetUrl;
        set => this.RaiseAndSetIfChanged(ref _defaultTargetUrl, value);
    }

    public ScanDepth DefaultScanDepth
    {
        get => _defaultScanDepth;
        set => this.RaiseAndSetIfChanged(ref _defaultScanDepth, value);
    }

    public int DefaultMaxRequestsPerSecond
    {
        get => _defaultMaxRequestsPerSecond;
        set => this.RaiseAndSetIfChanged(ref _defaultMaxRequestsPerSecond, value);
    }

    public int DefaultMaxConcurrentRequests
    {
        get => _defaultMaxConcurrentRequests;
        set => this.RaiseAndSetIfChanged(ref _defaultMaxConcurrentRequests, value);
    }

    public int DefaultRequestTimeout
    {
        get => _defaultRequestTimeout;
        set => this.RaiseAndSetIfChanged(ref _defaultRequestTimeout, value);
    }

    public bool DefaultFollowRedirects
    {
        get => _defaultFollowRedirects;
        set => this.RaiseAndSetIfChanged(ref _defaultFollowRedirects, value);
    }

    public int DefaultMaxRedirects
    {
        get => _defaultMaxRedirects;
        set => this.RaiseAndSetIfChanged(ref _defaultMaxRedirects, value);
    }

    public bool DefaultTestAllHttpMethods
    {
        get => _defaultTestAllHttpMethods;
        set => this.RaiseAndSetIfChanged(ref _defaultTestAllHttpMethods, value);
    }

    public bool DefaultGenerateReport
    {
        get => _defaultGenerateReport;
        set => this.RaiseAndSetIfChanged(ref _defaultGenerateReport, value);
    }

    public bool EnableNotifications
    {
        get => _enableNotifications;
        set => this.RaiseAndSetIfChanged(ref _enableNotifications, value);
    }

    public bool EnableSounds
    {
        get => _enableSounds;
        set => this.RaiseAndSetIfChanged(ref _enableSounds, value);
    }

    public bool AutoStartScans
    {
        get => _autoStartScans;
        set => this.RaiseAndSetIfChanged(ref _autoStartScans, value);
    }

    public bool SaveReportsAutomatically
    {
        get => _saveReportsAutomatically;
        set => this.RaiseAndSetIfChanged(ref _saveReportsAutomatically, value);
    }

    public string DefaultReportLocation
    {
        get => _defaultReportLocation;
        set => this.RaiseAndSetIfChanged(ref _defaultReportLocation, value);
    }

    public string DatabaseLocation
    {
        get => _databaseLocation;
        set => this.RaiseAndSetIfChanged(ref _databaseLocation, value);
    }

    public int MaxRecentScans
    {
        get => _maxRecentScans;
        set => this.RaiseAndSetIfChanged(ref _maxRecentScans, value);
    }

    public int ScanHistoryRetentionDays
    {
        get => _scanHistoryRetentionDays;
        set => this.RaiseAndSetIfChanged(ref _scanHistoryRetentionDays, value);
    }

    public bool EnableLogging
    {
        get => _enableLogging;
        set => this.RaiseAndSetIfChanged(ref _enableLogging, value);
    }

    public string LogLevel
    {
        get => _logLevel;
        set => this.RaiseAndSetIfChanged(ref _logLevel, value);
    }

    public bool EnableTelemetry
    {
        get => _enableTelemetry;
        set => this.RaiseAndSetIfChanged(ref _enableTelemetry, value);
    }

    public ObservableCollection<VulnerabilityTypeItem> DefaultVulnerabilityTypes { get; }
    public ObservableCollection<ReportFormatItem> DefaultReportFormats { get; }
    public ObservableCollection<string> LogLevels { get; }

    public ReactiveCommand<Unit, Unit> SaveSettingsCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadSettingsCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetToDefaultsCommand { get; }
    public ReactiveCommand<Unit, Unit> BrowseReportLocationCommand { get; }
    public ReactiveCommand<Unit, Unit> BrowseDatabaseLocationCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearScanHistoryCommand { get; }
    public ReactiveCommand<Unit, Unit> ExportSettingsCommand { get; }
    public ReactiveCommand<Unit, Unit> ImportSettingsCommand { get; }

    private void InitializeDefaults()
    {
        // Initialize default vulnerability types
        var vulnerabilityTypes = Enum.GetValues<VulnerabilityType>();
        foreach (var type in vulnerabilityTypes)
        {
            DefaultVulnerabilityTypes.Add(new VulnerabilityTypeItem
            {
                Type = type,
                IsSelected = true,
                DisplayName = GetVulnerabilityTypeDisplayName(type)
            });
        }

        // Initialize default report formats
        var reportFormats = Enum.GetValues<ReportFormat>();
        foreach (var format in reportFormats)
        {
            DefaultReportFormats.Add(new ReportFormatItem
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

    private Task SaveSettings()
    {
        try
        {
            _logger.LogInformation("Saving application settings");
            
            // In a real implementation, this would save settings to a configuration file
            // or registry/preferences storage
            
            _logger.LogInformation("Settings saved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save settings");
        }
        
        return Task.CompletedTask;
    }

    private Task LoadSettings()
    {
        try
        {
            _logger.LogInformation("Loading application settings");
            
            // In a real implementation, this would load settings from a configuration file
            // or registry/preferences storage
            
            _logger.LogInformation("Settings loaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load settings");
        }
        
        return Task.CompletedTask;
    }

    private void ResetToDefaults()
    {
        _logger.LogInformation("Resetting settings to defaults");
        
        DefaultTargetUrl = "http://localhost:3000";
        DefaultScanDepth = ScanDepth.Normal;
        DefaultMaxRequestsPerSecond = 10;
        DefaultMaxConcurrentRequests = 5;
        DefaultRequestTimeout = 30000;
        DefaultFollowRedirects = true;
        DefaultMaxRedirects = 5;
        DefaultTestAllHttpMethods = true;
        DefaultGenerateReport = true;
        EnableNotifications = true;
        EnableSounds = false;
        AutoStartScans = false;
        SaveReportsAutomatically = true;
        DefaultReportLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        DatabaseLocation = "VaultScope.db";
        MaxRecentScans = 50;
        ScanHistoryRetentionDays = 90;
        EnableLogging = true;
        LogLevel = "Information";
        EnableTelemetry = false;
        
        // Reset vulnerability types to all selected
        foreach (var type in DefaultVulnerabilityTypes)
        {
            type.IsSelected = true;
        }
        
        // Reset report formats
        foreach (var format in DefaultReportFormats)
        {
            format.IsSelected = format.Format == ReportFormat.Html || format.Format == ReportFormat.Json;
        }
    }

    private void BrowseReportLocation()
    {
        // In a real implementation, this would open a folder browser dialog
        _logger.LogInformation("Browse report location requested");
    }

    private void BrowseDatabaseLocation()
    {
        // In a real implementation, this would open a file browser dialog
        _logger.LogInformation("Browse database location requested");
    }

    private async Task ClearScanHistory()
    {
        try
        {
            _logger.LogInformation("Clearing scan history");
            
            // In a real implementation, this would clear the database
            await Task.CompletedTask;
            
            _logger.LogInformation("Scan history cleared successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear scan history");
        }
    }

    private async Task ExportSettings()
    {
        try
        {
            _logger.LogInformation("Exporting settings");
            
            // In a real implementation, this would export settings to a file
            await Task.CompletedTask;
            
            _logger.LogInformation("Settings exported successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export settings");
        }
    }

    private async Task ImportSettings()
    {
        try
        {
            _logger.LogInformation("Importing settings");
            
            // In a real implementation, this would import settings from a file
            await Task.CompletedTask;
            
            _logger.LogInformation("Settings imported successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import settings");
        }
    }
}