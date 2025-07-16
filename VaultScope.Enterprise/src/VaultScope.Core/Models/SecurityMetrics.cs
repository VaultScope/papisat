namespace VaultScope.Core.Models;

public class SecurityMetrics
{
    public int TotalScans { get; set; }
    
    public int TotalVulnerabilities { get; set; }
    
    public int HighSeverityCount { get; set; }
    
    public int MediumSeverityCount { get; set; }
    
    public int LowSeverityCount { get; set; }
    
    public double? SecurityScore { get; set; }
    
    public DateTime? LastScanDate { get; set; }
    
    public bool IsOperational { get; set; } = true;
    
    public Dictionary<VulnerabilityType, int> VulnerabilityTypeDistribution { get; set; } = new();
    
    public Dictionary<string, int> TargetUrlStats { get; set; } = new();
    
    public TimeSpan AverageScanDuration { get; set; }
    
    public double ScanSuccessRate { get; set; }
    
    public VulnerabilityStatus VulnerabilityStatus =>
        TotalVulnerabilities == 0 ? VulnerabilityStatus.None :
        TotalVulnerabilities <= 5 ? VulnerabilityStatus.Low :
        TotalVulnerabilities <= 15 ? VulnerabilityStatus.Medium :
        VulnerabilityStatus.High;
    
    public SecurityGrade SecurityGrade =>
        SecurityScore switch
        {
            >= 9.0 => SecurityGrade.Excellent,
            >= 8.0 => SecurityGrade.Good,
            >= 7.0 => SecurityGrade.Fair,
            >= 6.0 => SecurityGrade.Poor,
            _ => SecurityGrade.Critical
        };
}

public enum VulnerabilityStatus
{
    None,
    Low,
    Medium,
    High,
    Critical
}

public enum SecurityGrade
{
    Excellent,
    Good,
    Fair,
    Poor,
    Critical
}