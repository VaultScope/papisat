using System;

namespace VaultScope.Enterprise.WPF.Services;

/// <summary>
/// Implementation of the navigation service
/// </summary>
public class NavigationService : INavigationService
{
    private string _currentPage = "SecurityDashboard";
    
    public string CurrentPage => _currentPage;
    
    public event EventHandler<string>? NavigationChanged;
    
    public void NavigateTo(string pageName)
    {
        if (_currentPage != pageName)
        {
            _currentPage = pageName;
            NavigationChanged?.Invoke(this, pageName);
        }
    }
}