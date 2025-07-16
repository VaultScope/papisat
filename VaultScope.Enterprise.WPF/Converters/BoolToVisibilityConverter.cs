using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VaultScope.Enterprise.WPF.Converters;

/// <summary>
/// Converts boolean values to Visibility enum values
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean value to Visibility
    /// </summary>
    /// <param name="value">The boolean value to convert</param>
    /// <param name="targetType">The target type (should be Visibility)</param>
    /// <param name="parameter">Optional parameter to invert the conversion</param>
    /// <param name="culture">The culture to use for conversion</param>
    /// <returns>Visibility.Visible if true, Visibility.Collapsed if false</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            // Check if we should invert the conversion
            bool invert = parameter?.ToString()?.ToLower() == "invert";
            
            if (invert)
                boolValue = !boolValue;
            
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }
        
        return Visibility.Collapsed;
    }

    /// <summary>
    /// Converts a Visibility value back to boolean
    /// </summary>
    /// <param name="value">The Visibility value to convert</param>
    /// <param name="targetType">The target type (should be bool)</param>
    /// <param name="parameter">Optional parameter to invert the conversion</param>
    /// <param name="culture">The culture to use for conversion</param>
    /// <returns>true if Visible, false if Collapsed or Hidden</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            bool result = visibility == Visibility.Visible;
            
            // Check if we should invert the conversion
            bool invert = parameter?.ToString()?.ToLower() == "invert";
            
            if (invert)
                result = !result;
            
            return result;
        }
        
        return false;
    }
}