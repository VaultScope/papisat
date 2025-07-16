using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace VaultScope.Enterprise.WPF.Effects
{
    public class BackdropBlurEffect : ShaderEffect
    {
        public static readonly DependencyProperty BlurRadiusProperty =
            DependencyProperty.Register(nameof(BlurRadius), typeof(double), typeof(BackdropBlurEffect), 
                new PropertyMetadata(10.0, OnBlurRadiusChanged));

        public static readonly DependencyProperty InputProperty =
            RegisterPixelShaderSamplerProperty("Input", typeof(BackdropBlurEffect), 0);

        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register(nameof(Opacity), typeof(double), typeof(BackdropBlurEffect), 
                new PropertyMetadata(1.0, OnOpacityChanged));

        static BackdropBlurEffect()
        {
            // In a real implementation, you would load a custom pixel shader here
            // For now, we'll use a combination of built-in effects
        }

        public BackdropBlurEffect()
        {
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(BlurRadiusProperty);
            UpdateShaderValue(OpacityProperty);
        }

        public double BlurRadius
        {
            get => (double)GetValue(BlurRadiusProperty);
            set => SetValue(BlurRadiusProperty, value);
        }

        public Brush Input
        {
            get => (Brush)GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }

        public double Opacity
        {
            get => (double)GetValue(OpacityProperty);
            set => SetValue(OpacityProperty, value);
        }

        private static void OnBlurRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BackdropBlurEffect)d).UpdateShaderValue(BlurRadiusProperty);
        }

        private static void OnOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BackdropBlurEffect)d).UpdateShaderValue(OpacityProperty);
        }
    }

    public static class GlassEffectHelper
    {
        public static Effect CreateGlassEffect(double blurRadius = 40.0, double opacity = 0.8)
        {
            // Return a blur effect directly since WPF doesn't support effect groups
            var blurEffect = new BlurEffect
            {
                Radius = blurRadius
            };
            
            return blurEffect;
        }

        public static Brush CreateGlassBrush(Color baseColor, double opacity = 0.5)
        {
            var brush = new SolidColorBrush(baseColor)
            {
                Opacity = opacity
            };
            
            return brush;
        }

        public static LinearGradientBrush CreateGlassGradient(Color topColor, Color bottomColor, double opacity = 0.5)
        {
            var brush = new LinearGradientBrush(topColor, bottomColor, new Point(0, 0), new Point(0, 1))
            {
                Opacity = opacity
            };
            
            return brush;
        }

        public static void ApplyGlassEffect(FrameworkElement element, 
            double blurRadius = 40.0, 
            double opacity = 0.8,
            bool addShadow = true)
        {
            if (element == null) return;

            // Apply only blur effect since WPF doesn't support effect groups
            // Shadow can be added separately on a parent container if needed
            element.Effect = new BlurEffect { Radius = blurRadius };
            element.Opacity = opacity;
        }

        public static void RemoveGlassEffect(FrameworkElement element)
        {
            if (element == null) return;
            
            element.Effect = null;
            element.Opacity = 1.0;
        }
    }
}