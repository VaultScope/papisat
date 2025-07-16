using System;
using System.Windows;
using System.Windows.Controls;

namespace VaultScope.Enterprise.WPF.UserControls
{
    public partial class SidebarNavigationImproved : UserControl
    {
        public event EventHandler<string>? NavigationChanged;

        public SidebarNavigationImproved()
        {
            InitializeComponent();
        }

        private void SecurityDashboardButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationChanged?.Invoke(this, "Dashboard");
        }

        private void VulnerabilityScannerButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationChanged?.Invoke(this, "Scanner");
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationChanged?.Invoke(this, "Reports");
        }

        private void QuickScanButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationChanged?.Invoke(this, "QuickScan");
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationChanged?.Invoke(this, "Settings");
        }

        private void InitializeScanButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationChanged?.Invoke(this, "InitializeScan");
        }
    }
}