using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace VaultScope.Enterprise.WPF.Controls
{
    public class GlassPanel : ContentControl
    {
        public static readonly DependencyProperty GlassOpacityProperty =
            DependencyProperty.Register(nameof(GlassOpacity), typeof(double), typeof(GlassPanel), new PropertyMetadata(0.5));

        public static readonly DependencyProperty BlurRadiusProperty =
            DependencyProperty.Register(nameof(BlurRadius), typeof(double), typeof(GlassPanel), new PropertyMetadata(40.0));

        public static readonly DependencyProperty GlassBackgroundProperty =
            DependencyProperty.Register(nameof(GlassBackground), typeof(Brush), typeof(GlassPanel), new PropertyMetadata(null));

        public static readonly DependencyProperty BorderRadiusProperty =
            DependencyProperty.Register(nameof(BorderRadius), typeof(CornerRadius), typeof(GlassPanel), new PropertyMetadata(new CornerRadius(12)));

        public static readonly DependencyProperty HasShadowProperty =
            DependencyProperty.Register(nameof(HasShadow), typeof(bool), typeof(GlassPanel), new PropertyMetadata(true));

        public static readonly DependencyProperty ShadowDepthProperty =
            DependencyProperty.Register(nameof(ShadowDepth), typeof(double), typeof(GlassPanel), new PropertyMetadata(4.0));

        public static readonly DependencyProperty ShadowOpacityProperty =
            DependencyProperty.Register(nameof(ShadowOpacity), typeof(double), typeof(GlassPanel), new PropertyMetadata(0.25));

        public static readonly DependencyProperty HasBorderProperty =
            DependencyProperty.Register(nameof(HasBorder), typeof(bool), typeof(GlassPanel), new PropertyMetadata(true));

        public static readonly DependencyProperty GlassBorderBrushProperty =
            DependencyProperty.Register(nameof(GlassBorderBrush), typeof(Brush), typeof(GlassPanel), new PropertyMetadata(null));

        public static readonly DependencyProperty GlassBorderThicknessProperty =
            DependencyProperty.Register(nameof(GlassBorderThickness), typeof(Thickness), typeof(GlassPanel), new PropertyMetadata(new Thickness(1)));

        public static readonly DependencyProperty IsElevatedProperty =
            DependencyProperty.Register(nameof(IsElevated), typeof(bool), typeof(GlassPanel), new PropertyMetadata(false));

        static GlassPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GlassPanel), new FrameworkPropertyMetadata(typeof(GlassPanel)));
        }

        public double GlassOpacity
        {
            get => (double)GetValue(GlassOpacityProperty);
            set => SetValue(GlassOpacityProperty, value);
        }

        public double BlurRadius
        {
            get => (double)GetValue(BlurRadiusProperty);
            set => SetValue(BlurRadiusProperty, value);
        }

        public Brush GlassBackground
        {
            get => (Brush)GetValue(GlassBackgroundProperty);
            set => SetValue(GlassBackgroundProperty, value);
        }

        public CornerRadius BorderRadius
        {
            get => (CornerRadius)GetValue(BorderRadiusProperty);
            set => SetValue(BorderRadiusProperty, value);
        }

        public bool HasShadow
        {
            get => (bool)GetValue(HasShadowProperty);
            set => SetValue(HasShadowProperty, value);
        }

        public double ShadowDepth
        {
            get => (double)GetValue(ShadowDepthProperty);
            set => SetValue(ShadowDepthProperty, value);
        }

        public double ShadowOpacity
        {
            get => (double)GetValue(ShadowOpacityProperty);
            set => SetValue(ShadowOpacityProperty, value);
        }

        public bool HasBorder
        {
            get => (bool)GetValue(HasBorderProperty);
            set => SetValue(HasBorderProperty, value);
        }

        public Brush GlassBorderBrush
        {
            get => (Brush)GetValue(GlassBorderBrushProperty);
            set => SetValue(GlassBorderBrushProperty, value);
        }

        public Thickness GlassBorderThickness
        {
            get => (Thickness)GetValue(GlassBorderThicknessProperty);
            set => SetValue(GlassBorderThicknessProperty, value);
        }

        public bool IsElevated
        {
            get => (bool)GetValue(IsElevatedProperty);
            set => SetValue(IsElevatedProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateGlassEffect();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateGlassEffect();
        }

        private void UpdateGlassEffect()
        {
            if (GlassBackground == null)
            {
                var defaultBrush = TryFindResource("GlassGradientBrush") as Brush;
                if (defaultBrush != null)
                {
                    GlassBackground = defaultBrush;
                }
            }

            // Apply glass effect styling
            var border = GetTemplateChild("PART_GlassBorder") as Border;
            if (border != null)
            {
                border.Background = GlassBackground;
                border.Opacity = GlassOpacity;
                border.CornerRadius = BorderRadius;

                // Apply shadow effect
                if (HasShadow)
                {
                    var shadowEffect = IsElevated 
                        ? new DropShadowEffect
                        {
                            Color = Colors.Black,
                            Direction = 270,
                            ShadowDepth = ShadowDepth * 2,
                            BlurRadius = 32,
                            Opacity = ShadowOpacity + 0.05
                        }
                        : new DropShadowEffect
                        {
                            Color = Colors.Black,
                            Direction = 270,
                            ShadowDepth = ShadowDepth,
                            BlurRadius = 24,
                            Opacity = ShadowOpacity
                        };
                    border.Effect = shadowEffect;
                }

                // Apply border if enabled
                if (HasBorder)
                {
                    if (GlassBorderBrush == null)
                    {
                        var borderBrush = TryFindResource("BorderBrush") as Brush ?? new SolidColorBrush(Colors.Gray);
                        GlassBorderBrush = borderBrush;
                    }
                    border.BorderBrush = GlassBorderBrush;
                    border.BorderThickness = GlassBorderThickness;
                }
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            
            if (e.Property == GlassOpacityProperty ||
                e.Property == BlurRadiusProperty ||
                e.Property == GlassBackgroundProperty ||
                e.Property == BorderRadiusProperty ||
                e.Property == HasShadowProperty ||
                e.Property == ShadowDepthProperty ||
                e.Property == ShadowOpacityProperty ||
                e.Property == HasBorderProperty ||
                e.Property == GlassBorderBrushProperty ||
                e.Property == GlassBorderThicknessProperty ||
                e.Property == IsElevatedProperty)
            {
                UpdateGlassEffect();
            }
        }
    }
}