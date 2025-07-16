using System;
using System.Collections.Generic;

namespace VaultScope.Enterprise.WPF.Services;

/// <summary>
/// Service for handling theme changes
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Get the current theme
    /// </summary>
    string CurrentTheme { get; }
    
    /// <summary>
    /// Set the application theme
    /// </summary>
    /// <param name="themeName">The name of the theme to apply</param>
    void SetTheme(string themeName);
    
    /// <summary>
    /// Get available themes
    /// </summary>
    /// <returns>List of available theme names</returns>
    IEnumerable<string> GetAvailableThemes();
    
    /// <summary>
    /// Event raised when theme changes
    /// </summary>
    event EventHandler<string>? ThemeChanged;
}