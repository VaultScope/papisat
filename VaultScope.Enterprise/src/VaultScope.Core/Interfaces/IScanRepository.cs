using VaultScope.Core.Models;

namespace VaultScope.Core.Interfaces;

public interface IScanRepository
{
    Task<ScanResult> SaveScanResultAsync(ScanResult scanResult, CancellationToken cancellationToken = default);
    
    Task<ScanResult?> GetScanResultAsync(Guid scanId, CancellationToken cancellationToken = default);
    
    Task<List<ScanResult>> GetAllScanResultsAsync(CancellationToken cancellationToken = default);
    
    Task<List<ScanResult>> GetScanResultsByUrlAsync(string targetUrl, CancellationToken cancellationToken = default);
    
    Task<List<ScanResult>> GetRecentScanResultsAsync(int count = 10, CancellationToken cancellationToken = default);
    
    Task<bool> DeleteScanResultAsync(Guid scanId, CancellationToken cancellationToken = default);
    
    Task<long> GetTotalScansCountAsync(CancellationToken cancellationToken = default);
    
    Task<long> GetTotalVulnerabilitiesCountAsync(CancellationToken cancellationToken = default);
    
    Task<Dictionary<VulnerabilityType, int>> GetVulnerabilityStatisticsAsync(CancellationToken cancellationToken = default);
}