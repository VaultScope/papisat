using System;

namespace VaultScope.Enterprise.WPF.Services;

/// <summary>
/// Service for handling navigation between pages
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigate to a specific page
    /// </summary>
    /// <param name="pageName">The name of the page to navigate to</param>
    void NavigateTo(string pageName);
    
    /// <summary>
    /// Get the current page name
    /// </summary>
    string CurrentPage { get; }
    
    /// <summary>
    /// Event raised when navigation occurs
    /// </summary>
    event EventHandler<string>? NavigationChanged;
}