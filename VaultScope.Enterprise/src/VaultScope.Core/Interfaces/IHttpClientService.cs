using System.Net.Http;
using VaultScope.Core.Models;

namespace VaultScope.Core.Interfaces;

public interface IHttpClientService
{
    Task<HttpResponseMessage> SendRequestAsync(
        string url,
        HttpMethod method,
        Dictionary<string, string>? headers = null,
        string? content = null,
        string? contentType = "application/x-www-form-urlencoded",
        AuthenticationResult? authentication = null,
        CancellationToken cancellationToken = default);
    
    Task<HttpResponseMessage> GetAsync(
        string url, 
        Dictionary<string, string>? headers = null,
        AuthenticationResult? authentication = null,
        CancellationToken cancellationToken = default);
    
    Task<HttpResponseMessage> PostAsync(
        string url,
        string content,
        string contentType = "application/x-www-form-urlencoded",
        Dictionary<string, string>? headers = null,
        AuthenticationResult? authentication = null,
        CancellationToken cancellationToken = default);
    
    Task<bool> IsUrlAccessibleAsync(string url, CancellationToken cancellationToken = default);
    
    void SetTimeout(TimeSpan timeout);
    
    void SetMaxConcurrentRequests(int maxConcurrentRequests);
    
    void ConfigureProxy(string? proxyUrl, string? username = null, string? password = null);
}