namespace VaultScope.Core.Models;

public enum ScanStatus
{
    Pending,
    Initializing,
    Crawling,
    Testing,
    Analyzing,
    GeneratingReport,
    Completed,
    Failed,
    Cancelled,
    Timeout
}

public class ScanProgress
{
    public ScanStatus Status { get; set; } = ScanStatus.Pending;
    
    public double ProgressPercentage { get; set; } = 0.0;
    
    public string CurrentTask { get; set; } = string.Empty;
    
    public int EndpointsTested { get; set; } = 0;
    
    public int TotalEndpoints { get; set; } = 0;
    
    public int VulnerabilitiesFound { get; set; } = 0;
    
    public TimeSpan ElapsedTime { get; set; } = TimeSpan.Zero;
    
    public TimeSpan EstimatedTimeRemaining { get; set; } = TimeSpan.Zero;
    
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    
    public string? ErrorMessage { get; set; }
}