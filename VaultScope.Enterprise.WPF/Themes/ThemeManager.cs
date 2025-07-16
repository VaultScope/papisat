using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace VaultScope.Enterprise.WPF.Themes
{
    public enum ThemeType
    {
        Dark,
        Light,
        Auto
    }

    public class ThemeManager
    {
        private static readonly Lazy<ThemeManager> _instance = new(() => new ThemeManager());
        public static ThemeManager Instance => _instance.Value;

        private ThemeType _currentTheme = ThemeType.Dark;
        private readonly string _settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VaultScope", "theme.json");

        public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

        private ThemeManager()
        {
            LoadThemeSettings();
        }

        public ThemeType CurrentTheme
        {
            get => _currentTheme;
            set
            {
                if (_currentTheme != value)
                {
                    _currentTheme = value;
                    ApplyTheme();
                    SaveThemeSettings();
                    ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(value));
                }
            }
        }

        public void ApplyTheme()
        {
            var app = Application.Current;
            if (app == null) return;

            var themeToApply = _currentTheme == ThemeType.Auto ? GetSystemTheme() : _currentTheme;
            
            var existingTheme = app.Resources.MergedDictionaries.FirstOrDefault(d => 
                d.Source?.ToString().Contains("DarkTheme.xaml") == true || 
                d.Source?.ToString().Contains("LightTheme.xaml") == true);

            if (existingTheme != null)
            {
                app.Resources.MergedDictionaries.Remove(existingTheme);
            }

            var themeUri = themeToApply == ThemeType.Dark 
                ? new Uri("pack://application:,,,/Themes/DarkTheme.xaml", UriKind.Absolute)
                : new Uri("pack://application:,,,/Themes/LightTheme.xaml", UriKind.Absolute);

            var themeDict = new ResourceDictionary { Source = themeUri };
            app.Resources.MergedDictionaries.Add(themeDict);
        }

        private ThemeType GetSystemTheme()
        {
            try
            {
                var registry = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                var value = registry?.GetValue("AppsUseLightTheme");
                return value is int intValue && intValue == 1 ? ThemeType.Light : ThemeType.Dark;
            }
            catch
            {
                return ThemeType.Dark;
            }
        }

        private void LoadThemeSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    var settings = JsonSerializer.Deserialize<ThemeSettings>(json);
                    if (settings != null)
                    {
                        _currentTheme = settings.Theme;
                    }
                }
            }
            catch
            {
                _currentTheme = ThemeType.Dark;
            }
        }

        private void SaveThemeSettings()
        {
            try
            {
                var directory = Path.GetDirectoryName(_settingsPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }

                var settings = new ThemeSettings { Theme = _currentTheme };
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_settingsPath, json);
            }
            catch
            {
                // Ignore save errors
            }
        }

        public static Color GetAccentColor()
        {
            var resource = Application.Current?.FindResource("Purple500");
            return resource is Color color ? color : Colors.Purple;
        }

        public static SolidColorBrush GetAccentBrush()
        {
            var resource = Application.Current?.FindResource("Purple500Brush");
            return resource as SolidColorBrush ?? new SolidColorBrush(Colors.Purple);
        }
    }

    public class ThemeChangedEventArgs : EventArgs
    {
        public ThemeType NewTheme { get; }

        public ThemeChangedEventArgs(ThemeType newTheme)
        {
            NewTheme = newTheme;
        }
    }

    public class ThemeSettings
    {
        public ThemeType Theme { get; set; } = ThemeType.Dark;
    }
}