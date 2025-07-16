using System;
using System.Windows;
using System.Windows.Controls;

namespace VaultScope.Enterprise.WPF.Pages
{
    public partial class ComprehensiveScannerPage : UserControl
    {
        public ComprehensiveScannerPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void StartFullScanButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Starting comprehensive security scan...", "Scan Initiated", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void QuickScanButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Starting quick security scan...", "Quick Scan", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ResetConfigButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Reset all configuration to default values?", 
                "Reset Configuration", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                ResetConfiguration();
            }
        }

        private void ResetConfiguration()
        {
            TargetUrlTextBox.Text = string.Empty;
            ScanDepthComboBox.SelectedIndex = 1;
            ScanProfileComboBox.SelectedIndex = 0;
            MaxRequestsSlider.Value = 10;
            ConcurrentThreadsSlider.Value = 5;
            TimeoutSlider.Value = 30;
            CrawlDepthSlider.Value = 3;
        }
    }
}