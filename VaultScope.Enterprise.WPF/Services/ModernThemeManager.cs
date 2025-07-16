using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

namespace VaultScope.Enterprise.WPF.Services
{
    public class ModernThemeManager
    {
        private static ModernThemeManager? _instance;
        public static ModernThemeManager Instance => _instance ??= new ModernThemeManager();

        private ThemeMode _currentTheme = ThemeMode.Dark;
        private bool _isSystemDarkMode = true;

        public event EventHandler<ThemeMode>? ThemeChanged;

        private ModernThemeManager()
        {
            // Listen for system theme changes
            SystemEvents.UserPreferenceChanged += OnSystemPreferenceChanged;
            
            // Get initial system theme
            UpdateSystemThemeState();
            
            // Load saved theme preference
            LoadThemePreference();
        }

        public ThemeMode CurrentTheme
        {
            get => _currentTheme;
            set
            {
                if (_currentTheme != value)
                {
                    _currentTheme = value;
                    SaveThemePreference(value);
                    ApplyTheme();
                    ThemeChanged?.Invoke(this, value);
                }
            }
        }

        public bool IsCurrentlyDark => _currentTheme switch
        {
            ThemeMode.Dark => true,
            ThemeMode.Light => false,
            ThemeMode.System => _isSystemDarkMode,
            _ => true
        };

        public void ApplyTheme()
        {
            if (Application.Current?.Resources == null) return;

            var resources = Application.Current.Resources;
            var isDark = IsCurrentlyDark;

            // Update color resources based on theme
            UpdateColorResources(resources, isDark);
        }

        private void UpdateColorResources(ResourceDictionary resources, bool isDark)
        {
            if (isDark)
            {
                // Dark theme colors
                resources["BackgroundPrimary"] = Color.FromRgb(10, 10, 10);         // #FF0A0A0A
                resources["BackgroundSecondary"] = Color.FromRgb(18, 18, 18);       // #FF121212
                resources["BackgroundTertiary"] = Color.FromRgb(26, 26, 26);        // #FF1A1A1A
                resources["BackgroundElevated"] = Color.FromRgb(31, 31, 31);        // #FF1F1F1F
                resources["BackgroundGlass"] = Color.FromArgb(42, 31, 31, 31);      // #2A1F1F1F
                
                resources["TextPrimary"] = Color.FromRgb(255, 255, 255);            // #FFFFFFFF
                resources["TextSecondary"] = Color.FromRgb(180, 180, 180);          // #FFB4B4B4
                resources["TextTertiary"] = Color.FromRgb(115, 115, 115);           // #FF737373
                resources["TextDisabled"] = Color.FromRgb(74, 74, 74);              // #FF4A4A4A
                
                resources["BorderLight"] = Color.FromRgb(42, 42, 42);               // #FF2A2A2A
                resources["BorderMedium"] = Color.FromRgb(64, 64, 64);              // #FF404040
                resources["BorderDark"] = Color.FromRgb(26, 26, 26);                // #FF1A1A1A
                resources["BorderGlass"] = Color.FromArgb(51, 255, 255, 255);       // #33FFFFFF
                
                resources["GlassBackground"] = Color.FromArgb(26, 31, 31, 31);      // #1A1F1F1F
                resources["GlassBorder"] = Color.FromArgb(51, 255, 255, 255);       // #33FFFFFF
                resources["GlassHighlight"] = Color.FromArgb(26, 255, 255, 255);    // #1AFFFFFF
                resources["GlassShadow"] = Color.FromArgb(128, 0, 0, 0);            // #80000000
            }
            else
            {
                // Light theme colors
                resources["BackgroundPrimary"] = Color.FromRgb(255, 255, 255);      // #FFFFFFFF
                resources["BackgroundSecondary"] = Color.FromRgb(248, 250, 252);    // #FFF8FAFC
                resources["BackgroundTertiary"] = Color.FromRgb(241, 245, 249);     // #FFF1F5F9
                resources["BackgroundElevated"] = Color.FromRgb(255, 255, 255);     // #FFFFFFFF
                resources["BackgroundGlass"] = Color.FromArgb(230, 255, 255, 255);  // #E6FFFFFF
                
                resources["TextPrimary"] = Color.FromRgb(15, 23, 42);               // #FF0F172A
                resources["TextSecondary"] = Color.FromRgb(71, 85, 105);            // #FF475569
                resources["TextTertiary"] = Color.FromRgb(148, 163, 184);           // #FF94A3B8
                resources["TextDisabled"] = Color.FromRgb(203, 213, 225);           // #FFCBD5E1
                
                resources["BorderLight"] = Color.FromRgb(226, 232, 240);            // #FFE2E8F0
                resources["BorderMedium"] = Color.FromRgb(203, 213, 225);           // #FFCBD5E1
                resources["BorderDark"] = Color.FromRgb(148, 163, 184);             // #FF94A3B8
                resources["BorderGlass"] = Color.FromArgb(25, 0, 0, 0);             // #19000000
                
                resources["GlassBackground"] = Color.FromArgb(230, 255, 255, 255);  // #E6FFFFFF
                resources["GlassBorder"] = Color.FromArgb(25, 0, 0, 0);             // #19000000
                resources["GlassHighlight"] = Color.FromRgb(255, 255, 255);         // #FFFFFFFF
                resources["GlassShadow"] = Color.FromArgb(64, 0, 0, 0);             // #40000000
            }

            // Update brushes
            UpdateBrushResources(resources);
        }

        private void UpdateBrushResources(ResourceDictionary resources)
        {
            var colorKeys = new[]
            {
                "BackgroundPrimary", "BackgroundSecondary", "BackgroundTertiary", "BackgroundElevated",
                "TextPrimary", "TextSecondary", "TextTertiary", "TextDisabled",
                "BorderLight", "BorderMedium", "BorderDark", "BorderGlass",
                "GlassBackground", "GlassBorder", "GlassHighlight"
            };

            foreach (var key in colorKeys)
            {
                if (resources[key] is Color color)
                {
                    resources[$"{key}Brush"] = new SolidColorBrush(color);
                }
            }

            // Update gradient brushes
            if (resources["GlassBackground"] is Color glassColor)
            {
                var gradientBrush = new LinearGradientBrush();
                gradientBrush.StartPoint = new Point(0, 0);
                gradientBrush.EndPoint = new Point(0, 1);
                gradientBrush.GradientStops.Add(new GradientStop(glassColor, 0));
                gradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(10, glassColor.R, glassColor.G, glassColor.B), 1));
                resources["GlassGradientBrush"] = gradientBrush;
            }
        }

        private void OnSystemPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
            {
                Application.Current?.Dispatcher.BeginInvoke(() =>
                {
                    UpdateSystemThemeState();
                    if (_currentTheme == ThemeMode.System)
                    {
                        ApplyTheme();
                        ThemeChanged?.Invoke(this, _currentTheme);
                    }
                });
            }
        }

        private void UpdateSystemThemeState()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                var value = key?.GetValue("AppsUseLightTheme");
                _isSystemDarkMode = value is int intValue && intValue == 0;
            }
            catch
            {
                _isSystemDarkMode = true; // Default to dark
            }
        }

        private void LoadThemePreference()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\VaultScope");
                var value = key?.GetValue("ThemeMode");
                if (value is int intValue && Enum.IsDefined(typeof(ThemeMode), intValue))
                {
                    _currentTheme = (ThemeMode)intValue;
                }
            }
            catch
            {
                _currentTheme = ThemeMode.Dark; // Default to dark
            }
        }

        private void SaveThemePreference(ThemeMode theme)
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(@"Software\VaultScope");
                key?.SetValue("ThemeMode", (int)theme);
            }
            catch
            {
                // Ignore save errors
            }
        }

        public void Dispose()
        {
            SystemEvents.UserPreferenceChanged -= OnSystemPreferenceChanged;
        }
    }
}