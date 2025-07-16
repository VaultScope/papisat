using System.Windows;
using System.Windows.Controls;
using VaultScope.Enterprise.WPF.Services;

namespace VaultScope.Enterprise.WPF.UserControls
{
    public partial class ThemeSettingsPanel : UserControl
    {
        private readonly ThemeService _themeService;

        public ThemeSettingsPanel()
        {
            InitializeComponent();
            
            // Create theme service instance
            _themeService = new ThemeService();

            // Initialize UI based on current theme
            InitializeThemeSelection();
        }

        private void InitializeThemeSelection()
        {
            // Set the appropriate radio button based on current theme
            switch (_themeService.CurrentThemeMode)
            {
                case ThemeMode.Dark:
                    DarkThemeRadio.IsChecked = true;
                    break;
                case ThemeMode.Light:
                    LightThemeRadio.IsChecked = true;
                    break;
                case ThemeMode.System:
                    SystemThemeRadio.IsChecked = true;
                    break;
            }
        }

        private void ThemeRadio_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.Tag is string themeName)
            {
                if (System.Enum.TryParse<ThemeMode>(themeName, out var themeMode))
                {
                    _themeService.CurrentThemeMode = themeMode;
                }
            }
        }
    }
}