using CommunityToolkit.Mvvm.ComponentModel;

namespace VaultScope.Enterprise.WPF.ViewModels;

/// <summary>
/// Base view model class that provides INotifyPropertyChanged implementation
/// </summary>
public abstract partial class BaseViewModel : ObservableObject
{
    /// <summary>
    /// Indicates whether the view model is in a loading state
    /// </summary>
    [ObservableProperty]
    private bool isLoading;

    /// <summary>
    /// Contains any error message that should be displayed to the user
    /// </summary>
    [ObservableProperty]
    private string errorMessage = string.Empty;

    /// <summary>
    /// Indicates whether the view model has any errors
    /// </summary>
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    /// <summary>
    /// Clears any existing error message
    /// </summary>
    protected void ClearError()
    {
        ErrorMessage = string.Empty;
    }

    /// <summary>
    /// Sets an error message
    /// </summary>
    /// <param name="message">The error message to display</param>
    protected void SetError(string message)
    {
        ErrorMessage = message;
    }
}