using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace VaultScope.Enterprise.WPF.UserControls
{
    public partial class GlassmorphismSidebar : UserControl
    {
        public event EventHandler<string>? NavigationChanged;

        public GlassmorphismSidebar()
        {
            InitializeComponent();
            SetActiveButton("Dashboard");
        }

        private void DashboardButton_Click(object sender, MouseButtonEventArgs e)
        {
            SetActiveButton("Dashboard");
            NavigationChanged?.Invoke(this, "Dashboard");
            if (sender is FrameworkElement element)
                AddClickAnimation(element);
        }

        private void ScannerButton_Click(object sender, MouseButtonEventArgs e)
        {
            SetActiveButton("Scanner");
            NavigationChanged?.Invoke(this, "Scanner");
            if (sender is FrameworkElement element)
                AddClickAnimation(element);
        }

        private void ReportsButton_Click(object sender, MouseButtonEventArgs e)
        {
            SetActiveButton("Reports");
            NavigationChanged?.Invoke(this, "Reports");
            if (sender is FrameworkElement element)
                AddClickAnimation(element);
        }

        private void QuickScanButton_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationChanged?.Invoke(this, "QuickScan");
            if (sender is FrameworkElement element)
                AddClickAnimation(element);
        }

        private void SettingsButton_Click(object sender, MouseButtonEventArgs e)
        {
            SetActiveButton("Settings");
            NavigationChanged?.Invoke(this, "Settings");
            if (sender is FrameworkElement element)
                AddClickAnimation(element);
        }

        private void SetActiveButton(string activeButton)
        {
            // Reset all buttons to inactive style (solid dark)
            DashboardButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0x2D, 0x2D, 0x2D));
            ScannerButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0x2D, 0x2D, 0x2D));
            ReportsButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0x2D, 0x2D, 0x2D));

            // Set active button to purple
            switch (activeButton)
            {
                case "Dashboard":
                    DashboardButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0x8B, 0x5C, 0xF6));
                    break;
                case "Scanner":
                    ScannerButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0x8B, 0x5C, 0xF6));
                    break;
                case "Reports":
                    ReportsButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0x8B, 0x5C, 0xF6));
                    break;
            }
        }

        private void AddClickAnimation(FrameworkElement element)
        {
            // Animation removed for performance
        }
    }
}