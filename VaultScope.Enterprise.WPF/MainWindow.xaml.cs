using System.Windows;
using VaultScope.Enterprise.WPF.ViewModels;
using VaultScope.Enterprise.WPF.Pages;
using VaultScope.Enterprise.WPF.UserControls;

namespace VaultScope.Enterprise.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        
        // Delay initial navigation until after the window is loaded
        Loaded += (s, e) => NavigateToPageInternal("SecurityDashboard");
    }

    private void SidebarNavigation_NavigationChanged(object sender, NavigationChangedEventArgs e)
    {
        NavigateToPageInternal(e.PageName);
    }

    private void NavigateToPageInternal(string pageName)
    {
        FrameworkElement? page = pageName switch
        {
            "SecurityDashboard" => new SecurityDashboardPage(),
            "VulnerabilityScanner" => new ComprehensiveScannerPage(),
            "Reports" => new ReportsPage(),
            "Settings" => new SettingsPage(),
            _ => new SecurityDashboardPage()
        };

        if (page != null)
        {
            MainContentPresenter.Content = page;
            _viewModel.CurrentPage = pageName;
        }
    }

    public void NavigateToPage(string pageName)
    {
        NavigateToPageInternal(pageName);
        SidebarNavigation.SetActivePage(pageName);
    }
}