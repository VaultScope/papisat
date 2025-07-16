using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace VaultScope.Enterprise.WPF.Controls
{
    public class GlassCard : ContentControl
    {
        public static readonly DependencyProperty GlassOpacityProperty =
            DependencyProperty.Register(nameof(GlassOpacity), typeof(double), typeof(GlassCard), new PropertyMetadata(0.5));

        public static readonly DependencyProperty BlurRadiusProperty =
            DependencyProperty.Register(nameof(BlurRadius), typeof(double), typeof(GlassCard), new PropertyMetadata(40.0));

        public static readonly DependencyProperty GlassBackgroundProperty =
            DependencyProperty.Register(nameof(GlassBackground), typeof(Brush), typeof(GlassCard), new PropertyMetadata(null));

        public static readonly DependencyProperty ShadowDepthProperty =
            DependencyProperty.Register(nameof(ShadowDepth), typeof(double), typeof(GlassCard), new PropertyMetadata(4.0));

        public static readonly DependencyProperty ShadowOpacityProperty =
            DependencyProperty.Register(nameof(ShadowOpacity), typeof(double), typeof(GlassCard), new PropertyMetadata(0.25));

        public static readonly DependencyProperty BorderRadiusProperty =
            DependencyProperty.Register(nameof(BorderRadius), typeof(CornerRadius), typeof(GlassCard), new PropertyMetadata(new CornerRadius(16)));

        public static readonly DependencyProperty HasBorderProperty =
            DependencyProperty.Register(nameof(HasBorder), typeof(bool), typeof(GlassCard), new PropertyMetadata(true));

        public static readonly DependencyProperty IsHoverEnabledProperty =
            DependencyProperty.Register(nameof(IsHoverEnabled), typeof(bool), typeof(GlassCard), new PropertyMetadata(true));

        static GlassCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GlassCard), new FrameworkPropertyMetadata(typeof(GlassCard)));
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

        public CornerRadius BorderRadius
        {
            get => (CornerRadius)GetValue(BorderRadiusProperty);
            set => SetValue(BorderRadiusProperty, value);
        }

        public bool HasBorder
        {
            get => (bool)GetValue(HasBorderProperty);
            set => SetValue(HasBorderProperty, value);
        }

        public bool IsHoverEnabled
        {
            get => (bool)GetValue(IsHoverEnabledProperty);
            set => SetValue(IsHoverEnabledProperty, value);
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
                var shadowEffect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    Direction = 270,
                    ShadowDepth = ShadowDepth,
                    BlurRadius = 24,
                    Opacity = ShadowOpacity
                };
                border.Effect = shadowEffect;

                // Apply border if enabled
                if (HasBorder)
                {
                    var borderBrush = TryFindResource("BorderBrush") as Brush ?? new SolidColorBrush(Colors.Gray);
                    border.BorderBrush = borderBrush;
                    border.BorderThickness = new Thickness(1);
                }
            }
        }
    }
}