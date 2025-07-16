using System.Windows;
using System.Windows.Controls;
using VaultScope.Enterprise.WPF.ViewModels;

namespace VaultScope.Enterprise.WPF.Pages;

/// <summary>
/// Interaction logic for SecurityDashboardPage.xaml
/// </summary>
public partial class SecurityDashboardPage : UserControl
{
    private readonly SecurityDashboardViewModel _viewModel;

    public SecurityDashboardPage()
    {
        InitializeComponent();
        _viewModel = new SecurityDashboardViewModel();
        DataContext = _viewModel;
        
        // Update UI based on empty state
        UpdateEmptyState();
    }

    private void StartNewScan_Click(object sender, RoutedEventArgs e)
    {
        // Navigate to scanner page
        // This would typically be handled by the navigation service
        var mainWindow = Window.GetWindow(this) as MainWindow;
        mainWindow?.NavigateToPage("VulnerabilityScanner");
    }

    private void RefreshDashboard_Click(object sender, RoutedEventArgs e)
    {
        // Refresh dashboard data
        _viewModel.RefreshDashboardCommand.Execute(null);
    }

    private void StartSecurityScan_Click(object sender, RoutedEventArgs e)
    {
        // Start security scan
        _viewModel.StartSecurityScanCommand.Execute(null);
        UpdateEmptyState();
    }

    private void ViewDocumentation_Click(object sender, RoutedEventArgs e)
    {
        // Open documentation
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "https://docs.vaultscope.com",
            UseShellExecute = true
        });
    }

    private void ViewAllScans_Click(object sender, RoutedEventArgs e)
    {
        // Navigate to reports page
        var mainWindow = Window.GetWindow(this) as MainWindow;
        mainWindow?.NavigateToPage("Reports");
    }

    private void UpdateEmptyState()
    {
        // Show/hide empty state based on scan data
        if (_viewModel.IsEmptyState)
        {
            EmptyState.Visibility = Visibility.Visible;
            RecentScansSection.Visibility = Visibility.Collapsed;
        }
        else
        {
            EmptyState.Visibility = Visibility.Collapsed;
            RecentScansSection.Visibility = Visibility.Visible;
        }
    }
}