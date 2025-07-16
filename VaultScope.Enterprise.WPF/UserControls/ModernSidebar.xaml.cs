using System;
using System.Windows;
using System.Windows.Controls;
using VaultScope.Enterprise.WPF.Services;

namespace VaultScope.Enterprise.WPF.UserControls
{
    public partial class ModernSidebar : UserControl
    {
        public event EventHandler<string>? NavigationChanged;

        public ModernSidebar()
        {
            InitializeComponent();
            
            // Subscribe to theme changes to update button states
            ModernThemeManager.Instance.ThemeChanged += OnThemeChanged;
            
            // Set initial theme button states
            UpdateThemeButtonStates();
        }
        
        private void OnThemeChanged(object? sender, ThemeMode theme)
        {
            UpdateThemeButtonStates();
        }
        
        private void UpdateThemeButtonStates()
        {
            var currentTheme = ModernThemeManager.Instance.CurrentTheme;
            
            // Reset all theme buttons to default style
            LightThemeButton.Style = (Style)FindResource("ModernThemeButton");
            DarkThemeButton.Style = (Style)FindResource("ModernThemeButton");
            SystemThemeButton.Style = (Style)FindResource("ModernThemeButton");
            
            // Set active style and update status text for current theme
            switch (currentTheme)
            {
                case ThemeMode.Light:
                    LightThemeButton.Style = (Style)FindResource("ActiveThemeButton");
                    ThemeStatusText.Text = "☀️ Light Mode Active";
                    break;
                case ThemeMode.Dark:
                    DarkThemeButton.Style = (Style)FindResource("ActiveThemeButton");
                    ThemeStatusText.Text = "🌙 Dark Mode Active";
                    break;
                case ThemeMode.System:
                    SystemThemeButton.Style = (Style)FindResource("ActiveThemeButton");
                    ThemeStatusText.Text = "🖥️ System Theme Active";
                    break;
            }
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
        
        private void LightThemeButton_Click(object sender, RoutedEventArgs e)
        {
            ModernThemeManager.Instance.CurrentTheme = ThemeMode.Light;
        }
        
        private void DarkThemeButton_Click(object sender, RoutedEventArgs e)
        {
            ModernThemeManager.Instance.CurrentTheme = ThemeMode.Dark;
        }
        
        private void SystemThemeButton_Click(object sender, RoutedEventArgs e)
        {
            ModernThemeManager.Instance.CurrentTheme = ThemeMode.System;
        }
    }
}