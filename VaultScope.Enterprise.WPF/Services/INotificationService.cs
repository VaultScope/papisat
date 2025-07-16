namespace VaultScope.Enterprise.WPF.Services;

/// <summary>
/// Service for handling notifications
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Show an information notification
    /// </summary>
    /// <param name="message">The message to display</param>
    void ShowInfo(string message);
    
    /// <summary>
    /// Show a success notification
    /// </summary>
    /// <param name="message">The message to display</param>
    void ShowSuccess(string message);
    
    /// <summary>
    /// Show a warning notification
    /// </summary>
    /// <param name="message">The message to display</param>
    void ShowWarning(string message);
    
    /// <summary>
    /// Show an error notification
    /// </summary>
    /// <param name="message">The message to display</param>
    void ShowError(string message);
}