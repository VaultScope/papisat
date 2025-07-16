using System.Linq;
using Microsoft.EntityFrameworkCore;
using VaultScope.Core.Interfaces;
using VaultScope.Core.Models;
using VaultScope.Infrastructure.Data.Entities;

namespace VaultScope.Infrastructure.Data.Repositories;

public class ScanRepository : IScanRepository
{
    private readonly VaultScopeDbContext _context;
    
    public ScanRepository(VaultScopeDbContext context)
    {
        _context = context;
    }
    
    public async Task<ScanResult> SaveScanResultAsync(ScanResult scanResult, CancellationToken cancellationToken = default)
    {
        var entity = ScanResultEntity.FromDomainModel(scanResult);
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }
        
        _context.ScanResults.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        scanResult.Id = entity.Id;
        return scanResult;
    }
    
    public async Task<ScanResult?> GetScanResultAsync(Guid scanId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ScanResults
            .Include(s => s.Vulnerabilities)
            .Include(s => s.SecurityScore)
            .FirstOrDefaultAsync(s => s.Id == scanId, cancellationToken);
        
        if (entity == null)
            return null;
        
        return MapToScanResult(entity);
    }
    
    public async Task<List<ScanResult>> GetAllScanResultsAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.ScanResults
            .Include(s => s.Vulnerabilities)
            .Include(s => s.SecurityScore)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync(cancellationToken);
        
        return entities.Select(MapToScanResult).ToList();
    }
    
    public async Task<List<ScanResult>> GetScanResultsByUrlAsync(string targetUrl, CancellationToken cancellationToken = default)
    {
        var entities = await _context.ScanResults
            .Include(s => s.Vulnerabilities)
            .Include(s => s.SecurityScore)
            .Where(s => s.TargetUrl == targetUrl)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync(cancellationToken);
        
        return entities.Select(MapToScanResult).ToList();
    }
    
    public async Task<List<ScanResult>> GetRecentScanResultsAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        var entities = await _context.ScanResults
            .Include(s => s.Vulnerabilities)
            .Include(s => s.SecurityScore)
            .OrderByDescending(s => s.StartTime)
            .Take(count)
            .ToListAsync(cancellationToken);
        
        return entities.Select(MapToScanResult).ToList();
    }
    
    public async Task<bool> DeleteScanResultAsync(Guid scanId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ScanResults.FindAsync(new object[] { scanId }, cancellationToken);
        if (entity == null)
            return false;
        
        _context.ScanResults.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
    
    public async Task<long> GetTotalScansCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ScanResults.CountAsync(cancellationToken);
    }
    
    public async Task<long> GetTotalVulnerabilitiesCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Vulnerabilities.CountAsync(cancellationToken);
    }
    
    public async Task<Dictionary<VulnerabilityType, int>> GetVulnerabilityStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var stats = await _context.Vulnerabilities
            .GroupBy(v => v.Type)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);
        
        var result = new Dictionary<VulnerabilityType, int>();
        foreach (var stat in stats)
        {
            if (Enum.TryParse<VulnerabilityType>(stat.Type, out var type))
            {
                result[type] = stat.Count;
            }
        }
        
        return result;
    }
    
    private ScanResult MapToScanResult(ScanResultEntity entity)
    {
        return entity.ToDomainModel();
    }
}