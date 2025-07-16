using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace VaultScope.Enterprise.WPF.ViewModels;

/// <summary>
/// View model for the Settings & Configuration page
/// </summary>
public partial class SettingsViewModel : BaseViewModel
{
    public SettingsViewModel()
    {
        // Initialize with default values
        LoadDefaultSettings();
        
        // Initialize scan profiles
        ScanProfiles = new ObservableCollection<string>
        {
            "Quick Scan",
            "Comprehensive Scan",
            "Custom Scan",
            "OWASP Top 10",
            "Penetration Test"
        };
        
        // Initialize notification types
        NotificationTypes = new ObservableCollection<string>
        {
            "Email",
            "Desktop Notification",
            "System Tray",
            "Sound Alert"
        };
        
        // Initialize themes
        AvailableThemes = new ObservableCollection<string>
        {
            "Dark",
            "Light",
            "Auto (System)"
        };
    }

    #region Collections
    
    /// <summary>
    /// Available themes
    /// </summary>
    public ObservableCollection<string> AvailableThemes { get; set; } = new();
    
    #endregion

    #region General Settings
    
    /// <summary>
    /// Enable automatic updates
    /// </summary>
    [ObservableProperty]
    private bool enableAutomaticUpdates;

    /// <summary>
    /// Enable telemetry and analytics
    /// </summary>
    [ObservableProperty]
    private bool enableTelemetry;

    /// <summary>
    /// Selected application theme
    /// </summary>
    [ObservableProperty]
    private string selectedTheme = "Dark";


    /// <summary>
    /// Startup behavior setting
    /// </summary>
    [ObservableProperty]
    private bool startWithWindows;

    /// <summary>
    /// Minimize to system tray
    /// </summary>
    [ObservableProperty]
    private bool minimizeToTray;

    #endregion

    #region Security Settings

    /// <summary>
    /// Maximum number of concurrent scans
    /// </summary>
    [ObservableProperty]
    private int maxConcurrentScans = 5;

    /// <summary>
    /// Request timeout in seconds
    /// </summary>
    [ObservableProperty]
    private int requestTimeout = 30;

    /// <summary>
    /// Default scan profile
    /// </summary>
    [ObservableProperty]
    private string defaultScanProfile = "Quick Scan";

    /// <summary>
    /// Available scan profiles
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<string> scanProfiles;

    /// <summary>
    /// Enable SSL/TLS verification
    /// </summary>
    [ObservableProperty]
    private bool enableSslVerification = true;

    /// <summary>
    /// Follow redirects during scans
    /// </summary>
    [ObservableProperty]
    private bool followRedirects = true;

    /// <summary>
    /// Enable rate limiting
    /// </summary>
    [ObservableProperty]
    private bool enableRateLimit = true;

    /// <summary>
    /// Rate limit requests per second
    /// </summary>
    [ObservableProperty]
    private int rateLimitRps = 10;

    #endregion

    #region Notification Settings

    /// <summary>
    /// Enable scan completion notifications
    /// </summary>
    [ObservableProperty]
    private bool enableScanNotifications = true;

    /// <summary>
    /// Enable vulnerability alert notifications
    /// </summary>
    [ObservableProperty]
    private bool enableVulnerabilityAlerts = true;

    /// <summary>
    /// Enable error notifications
    /// </summary>
    [ObservableProperty]
    private bool enableErrorNotifications = true;

    /// <summary>
    /// Selected notification type
    /// </summary>
    [ObservableProperty]
    private string selectedNotificationType = "Desktop Notification";

    /// <summary>
    /// Available notification types
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<string> notificationTypes;

    /// <summary>
    /// Email address for notifications
    /// </summary>
    [ObservableProperty]
    private string notificationEmail = string.Empty;

    #endregion

    #region Data & Privacy Settings

    /// <summary>
    /// Enable data encryption
    /// </summary>
    [ObservableProperty]
    private bool enableDataEncryption = true;

    /// <summary>
    /// Automatically delete old scan results
    /// </summary>
    [ObservableProperty]
    private bool autoDeleteOldResults = false;

    /// <summary>
    /// Days to keep scan results
    /// </summary>
    [ObservableProperty]
    private int daysToKeepResults = 30;

    /// <summary>
    /// Enable audit logging
    /// </summary>
    [ObservableProperty]
    private bool enableAuditLogging = true;

    /// <summary>
    /// Export scan data anonymously
    /// </summary>
    [ObservableProperty]
    private bool exportDataAnonymously = false;

    #endregion

    #region Advanced Settings

    /// <summary>
    /// Enable debug logging
    /// </summary>
    [ObservableProperty]
    private bool enableDebugLogging = false;

    /// <summary>
    /// Custom user agent string
    /// </summary>
    [ObservableProperty]
    private string customUserAgent = string.Empty;

    /// <summary>
    /// API endpoint URL
    /// </summary>
    [ObservableProperty]
    private string apiEndpoint = "https://api.vaultscope.com";

    /// <summary>
    /// API key for authentication
    /// </summary>
    [ObservableProperty]
    private string apiKey = string.Empty;

    /// <summary>
    /// Proxy server URL
    /// </summary>
    [ObservableProperty]
    private string proxyUrl = string.Empty;

    /// <summary>
    /// Enable proxy authentication
    /// </summary>
    [ObservableProperty]
    private bool enableProxyAuth = false;

    /// <summary>
    /// Proxy username
    /// </summary>
    [ObservableProperty]
    private string proxyUsername = string.Empty;

    /// <summary>
    /// Proxy password
    /// </summary>
    [ObservableProperty]
    private string proxyPassword = string.Empty;

    #endregion

    /// <summary>
    /// Indicates whether settings have been modified
    /// </summary>
    [ObservableProperty]
    private bool hasUnsavedChanges;

    /// <summary>
    /// Command to save settings
    /// </summary>
    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        IsLoading = true;
        ClearError();
        
        try
        {
            // TODO: Implement actual settings persistence
            await Task.Delay(1000); // Simulate save operation
            
            // Validate settings
            if (!ValidateSettings())
            {
                return;
            }
            
            // Save settings to configuration file or database
            // This would be implemented with actual persistence logic
            
            HasUnsavedChanges = false;
        }
        catch (Exception ex)
        {
            SetError($"Failed to save settings: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Command to reset settings to defaults
    /// </summary>
    [RelayCommand]
    private void ResetToDefaults()
    {
        LoadDefaultSettings();
        HasUnsavedChanges = true;
    }

    /// <summary>
    /// Command to test API connection
    /// </summary>
    [RelayCommand]
    private async Task TestApiConnectionAsync()
    {
        IsLoading = true;
        ClearError();
        
        try
        {
            // TODO: Implement actual API connection test
            await Task.Delay(2000); // Simulate API test
            
            // Test API connection with current settings
            // This would make actual HTTP request to API endpoint
        }
        catch (Exception ex)
        {
            SetError($"API connection test failed: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Command to export settings
    /// </summary>
    [RelayCommand]
    private async Task ExportSettingsAsync()
    {
        IsLoading = true;
        ClearError();
        
        try
        {
            // TODO: Implement settings export
            await Task.Delay(1000); // Simulate export operation
            
            // Export settings to JSON file
        }
        catch (Exception ex)
        {
            SetError($"Failed to export settings: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Command to import settings
    /// </summary>
    [RelayCommand]
    private async Task ImportSettingsAsync()
    {
        IsLoading = true;
        ClearError();
        
        try
        {
            // TODO: Implement settings import
            await Task.Delay(1000); // Simulate import operation
            
            // Import settings from JSON file
            HasUnsavedChanges = true;
        }
        catch (Exception ex)
        {
            SetError($"Failed to import settings: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Loads default settings values
    /// </summary>
    private void LoadDefaultSettings()
    {
        // General Settings
        EnableAutomaticUpdates = true;
        EnableTelemetry = false;
        SelectedTheme = "Dark";
        StartWithWindows = false;
        MinimizeToTray = true;

        // Security Settings
        MaxConcurrentScans = 5;
        RequestTimeout = 30;
        DefaultScanProfile = "Quick Scan";
        EnableSslVerification = true;
        FollowRedirects = true;
        EnableRateLimit = true;
        RateLimitRps = 10;

        // Notification Settings
        EnableScanNotifications = true;
        EnableVulnerabilityAlerts = true;
        EnableErrorNotifications = true;
        SelectedNotificationType = "Desktop Notification";
        NotificationEmail = string.Empty;

        // Data & Privacy Settings
        EnableDataEncryption = true;
        AutoDeleteOldResults = false;
        DaysToKeepResults = 30;
        EnableAuditLogging = true;
        ExportDataAnonymously = false;

        // Advanced Settings
        EnableDebugLogging = false;
        CustomUserAgent = string.Empty;
        ApiEndpoint = "https://api.vaultscope.com";
        ApiKey = string.Empty;
        ProxyUrl = string.Empty;
        EnableProxyAuth = false;
        ProxyUsername = string.Empty;
        ProxyPassword = string.Empty;

        HasUnsavedChanges = false;
    }

    /// <summary>
    /// Validates current settings
    /// </summary>
    /// <returns>True if settings are valid, false otherwise</returns>
    private bool ValidateSettings()
    {
        // Validate numeric ranges
        if (MaxConcurrentScans < 1 || MaxConcurrentScans > 50)
        {
            SetError("Maximum concurrent scans must be between 1 and 50");
            return false;
        }

        if (RequestTimeout < 5 || RequestTimeout > 300)
        {
            SetError("Request timeout must be between 5 and 300 seconds");
            return false;
        }

        if (RateLimitRps < 1 || RateLimitRps > 1000)
        {
            SetError("Rate limit must be between 1 and 1000 requests per second");
            return false;
        }

        if (DaysToKeepResults < 1 || DaysToKeepResults > 365)
        {
            SetError("Days to keep results must be between 1 and 365");
            return false;
        }

        // Validate email format
        if (!string.IsNullOrEmpty(NotificationEmail) && !IsValidEmail(NotificationEmail))
        {
            SetError("Invalid email address format");
            return false;
        }

        // Validate API endpoint URL
        if (!string.IsNullOrEmpty(ApiEndpoint) && !Uri.TryCreate(ApiEndpoint, UriKind.Absolute, out _))
        {
            SetError("Invalid API endpoint URL format");
            return false;
        }

        // Validate proxy URL if provided
        if (!string.IsNullOrEmpty(ProxyUrl) && !Uri.TryCreate(ProxyUrl, UriKind.Absolute, out _))
        {
            SetError("Invalid proxy URL format");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validates email address format
    /// </summary>
    /// <param name="email">Email address to validate</param>
    /// <returns>True if email is valid, false otherwise</returns>
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    partial void OnMaxConcurrentScansChanged(int value)
    {
        HasUnsavedChanges = true;
    }

    partial void OnRequestTimeoutChanged(int value)
    {
        HasUnsavedChanges = true;
    }

    partial void OnSelectedThemeChanged(string value)
    {
        HasUnsavedChanges = true;
    }

    partial void OnEnableAutomaticUpdatesChanged(bool value)
    {
        HasUnsavedChanges = true;
    }

    partial void OnEnableTelemetryChanged(bool value)
    {
        HasUnsavedChanges = true;
    }
}