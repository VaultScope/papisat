using System;
using System.Threading.Tasks;

namespace VaultScope.Enterprise.WPF.Services;

/// <summary>
/// Service for handling security scans
/// </summary>
public interface ISecurityScanService
{
    /// <summary>
    /// Start a security scan
    /// </summary>
    /// <param name="targetUrl">The URL to scan</param>
    /// <param name="scanProfile">The scan profile to use</param>
    /// <returns>Task representing the scan operation</returns>
    Task<bool> StartScanAsync(string targetUrl, string scanProfile);
    
    /// <summary>
    /// Stop the current scan
    /// </summary>
    void StopScan();
    
    /// <summary>
    /// Check if a scan is currently running
    /// </summary>
    bool IsScanRunning { get; }
    
    /// <summary>
    /// Event raised when scan progress changes
    /// </summary>
    event EventHandler<int>? ScanProgressChanged;
    
    /// <summary>
    /// Event raised when scan status changes
    /// </summary>
    event EventHandler<string>? ScanStatusChanged;
}