using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

namespace VaultScope.Enterprise.WPF.Services
{
    public enum ThemeMode
    {
        Dark,
        Light,
        System
    }

    /// <summary>
    /// Implementation of the theme service with Dark/Light/System modes
    /// </summary>
    public class ThemeService : IThemeService, INotifyPropertyChanged
    {
        private ThemeMode _currentThemeMode = ThemeMode.System;
        private bool _isDarkTheme = true;
        private readonly string _settingsKey = "VaultScope_ThemeMode";

        public string CurrentTheme => _currentThemeMode.ToString();
        
        public ThemeMode CurrentThemeMode
        {
            get => _currentThemeMode;
            set
            {
                if (_currentThemeMode != value)
                {
                    _currentThemeMode = value;
                    SaveThemePreference(value);
                    ApplyTheme(value);
                    OnPropertyChanged();
                    ThemeChanged?.Invoke(this, value.ToString());
                }
            }
        }

        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            private set
            {
                if (_isDarkTheme != value)
                {
                    _isDarkTheme = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public event EventHandler<string>? ThemeChanged;

        public void InitializeTheme()
        {
            // Load saved theme preference
            var savedTheme = LoadThemePreference();
            _currentThemeMode = savedTheme;
            
            // Apply the theme
            ApplyTheme(savedTheme);
            
            // Listen for system theme changes
            SystemEvents.UserPreferenceChanged += OnSystemThemeChanged;
        }

        public void SetTheme(string themeName)
        {
            if (Enum.TryParse<ThemeMode>(themeName, out var mode))
            {
                CurrentThemeMode = mode;
            }
        }

        public void ApplyTheme(ThemeMode mode)
        {
            bool shouldUseDarkTheme;
            
            switch (mode)
            {
                case ThemeMode.Dark:
                    shouldUseDarkTheme = true;
                    break;
                case ThemeMode.Light:
                    shouldUseDarkTheme = false;
                    break;
                case ThemeMode.System:
                default:
                    shouldUseDarkTheme = IsSystemDarkTheme();
                    break;
            }

            IsDarkTheme = shouldUseDarkTheme;
            
            // Apply the theme to the application
            var app = Application.Current;
            if (app != null)
            {
                var themeUri = new Uri(
                    shouldUseDarkTheme 
                        ? "/Themes/PurpleDarkTheme.xaml" 
                        : "/Themes/PurpleLightTheme.xaml", 
                    UriKind.Relative);
                
                // Remove existing theme dictionaries
                for (int i = app.Resources.MergedDictionaries.Count - 1; i >= 0; i--)
                {
                    var dict = app.Resources.MergedDictionaries[i];
                    if (dict.Source != null && dict.Source.OriginalString.Contains("Theme.xaml"))
                    {
                        app.Resources.MergedDictionaries.RemoveAt(i);
                    }
                }
                
                // Add new theme
                var themeDict = new ResourceDictionary { Source = themeUri };
                app.Resources.MergedDictionaries.Add(themeDict);
            }
        }

        private bool IsSystemDarkTheme()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        var value = key.GetValue("AppsUseLightTheme");
                        if (value is int intValue)
                        {
                            return intValue == 0; // 0 means dark theme
                        }
                    }
                }
            }
            catch
            {
                // Default to dark theme if we can't read the registry
            }
            
            return true;
        }

        private void OnSystemThemeChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General && _currentThemeMode == ThemeMode.System)
            {
                ApplyTheme(ThemeMode.System);
            }
        }

        private ThemeMode LoadThemePreference()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\VaultScope"))
                {
                    var value = key?.GetValue(_settingsKey);
                    if (value is int intValue && Enum.IsDefined(typeof(ThemeMode), intValue))
                    {
                        return (ThemeMode)intValue;
                    }
                }
            }
            catch
            {
                // Default to System if we can't read the preference
            }
            
            return ThemeMode.System;
        }

        private void SaveThemePreference(ThemeMode mode)
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\VaultScope"))
                {
                    key?.SetValue(_settingsKey, (int)mode);
                }
            }
            catch
            {
                // Ignore save errors
            }
        }

        public IEnumerable<string> GetAvailableThemes()
        {
            return new[] { "Dark", "Light", "System" };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}