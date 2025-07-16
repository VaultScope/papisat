using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VaultScope.Enterprise.WPF.UserControls;

/// <summary>
/// Interaction logic for SidebarNavigation.xaml
/// </summary>
public partial class SidebarNavigation : UserControl
{
    public event EventHandler<NavigationChangedEventArgs>? NavigationChanged;

    public SidebarNavigation()
    {
        InitializeComponent();
    }

    private void SecurityDashboardButton_Click(object sender, RoutedEventArgs e)
    {
        SetActiveButton("SecurityDashboard");
        NavigationChanged?.Invoke(this, new NavigationChangedEventArgs("SecurityDashboard"));
    }

    private void VulnerabilityScannerButton_Click(object sender, RoutedEventArgs e)
    {
        SetActiveButton("VulnerabilityScanner");
        NavigationChanged?.Invoke(this, new NavigationChangedEventArgs("VulnerabilityScanner"));
    }

    private void ReportsButton_Click(object sender, RoutedEventArgs e)
    {
        SetActiveButton("Reports");
        NavigationChanged?.Invoke(this, new NavigationChangedEventArgs("Reports"));
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        SetActiveButton("Settings");
        NavigationChanged?.Invoke(this, new NavigationChangedEventArgs("Settings"));
    }

    private void QuickScanButton_Click(object sender, RoutedEventArgs e)
    {
        SetActiveButton("VulnerabilityScanner");
        NavigationChanged?.Invoke(this, new NavigationChangedEventArgs("VulnerabilityScanner"));
    }

    private void InitializeScanButton_Click(object sender, RoutedEventArgs e)
    {
        SetActiveButton("VulnerabilityScanner");
        NavigationChanged?.Invoke(this, new NavigationChangedEventArgs("VulnerabilityScanner"));
    }

    private void SetActiveButton(string activePage)
    {
        // Reset all buttons to inactive state
        SecurityDashboardButton.Style = (Style)FindResource("NavigationButtonStyle");
        VulnerabilityScannerButton.Style = (Style)FindResource("NavigationButtonStyle");
        ReportsButton.Style = (Style)FindResource("NavigationButtonStyle");
        SettingsButton.Style = (Style)FindResource("NavigationButtonStyle");

        // Set the active button
        Button? activeButton = activePage switch
        {
            "SecurityDashboard" => SecurityDashboardButton,
            "VulnerabilityScanner" => VulnerabilityScannerButton,
            "Reports" => ReportsButton,
            "Settings" => SettingsButton,
            _ => SecurityDashboardButton
        };

        if (activeButton != null)
        {
            activeButton.Style = (Style)FindResource("NavigationButtonActiveStyle");
        }

        // Update icon colors
        UpdateIconColors(activePage);
    }

    private void UpdateIconColors(string activePage)
    {
        // Reset all icons to inactive color
        var inactiveColor = (System.Windows.Media.Brush)FindResource("TextTertiaryBrush");
        var activeColor = (System.Windows.Media.Brush)FindResource("TextPrimaryBrush");

        // Update icon colors based on active page
        if (SecurityDashboardButton.Content is Grid dashboardGrid)
        {
            var icon = dashboardGrid.Children.OfType<TextBlock>().FirstOrDefault();
            if (icon != null)
            {
                icon.Foreground = activePage == "SecurityDashboard" ? activeColor : inactiveColor;
            }
        }

        if (VulnerabilityScannerButton.Content is Grid scannerGrid)
        {
            var icon = scannerGrid.Children.OfType<TextBlock>().FirstOrDefault();
            if (icon != null)
            {
                icon.Foreground = activePage == "VulnerabilityScanner" ? activeColor : inactiveColor;
            }
        }

        if (ReportsButton.Content is Grid reportsGrid)
        {
            var icon = reportsGrid.Children.OfType<TextBlock>().FirstOrDefault();
            if (icon != null)
            {
                icon.Foreground = activePage == "Reports" ? activeColor : inactiveColor;
            }
        }

        if (SettingsButton.Content is Grid settingsGrid)
        {
            var icon = settingsGrid.Children.OfType<TextBlock>().FirstOrDefault();
            if (icon != null)
            {
                icon.Foreground = activePage == "Settings" ? activeColor : inactiveColor;
            }
        }
    }

    public void SetActivePage(string pageName)
    {
        SetActiveButton(pageName);
    }
}

/// <summary>
/// Event args for navigation changes
/// </summary>
public class NavigationChangedEventArgs : EventArgs
{
    public string PageName { get; }

    public NavigationChangedEventArgs(string pageName)
    {
        PageName = pageName;
    }
}