using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace VaultScope.Enterprise.WPF.Controls
{
    public class GlassButton : Button
    {
        public static readonly DependencyProperty GlassOpacityProperty =
            DependencyProperty.Register(nameof(GlassOpacity), typeof(double), typeof(GlassButton), new PropertyMetadata(0.5));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(object), typeof(GlassButton), new PropertyMetadata(null));

        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(GlassButton), new PropertyMetadata(16.0));

        public static readonly DependencyProperty BorderRadiusProperty =
            DependencyProperty.Register(nameof(BorderRadius), typeof(CornerRadius), typeof(GlassButton), new PropertyMetadata(new CornerRadius(8)));

        public static readonly DependencyProperty GlassBackgroundProperty =
            DependencyProperty.Register(nameof(GlassBackground), typeof(Brush), typeof(GlassButton), new PropertyMetadata(null));

        public static readonly DependencyProperty ButtonTypeProperty =
            DependencyProperty.Register(nameof(ButtonType), typeof(GlassButtonType), typeof(GlassButton), new PropertyMetadata(GlassButtonType.Default));

        private Storyboard? _hoverInStoryboard;
        private Storyboard? _hoverOutStoryboard;
        private Storyboard? _pressStoryboard;

        static GlassButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GlassButton), new FrameworkPropertyMetadata(typeof(GlassButton)));
        }

        public double GlassOpacity
        {
            get => (double)GetValue(GlassOpacityProperty);
            set => SetValue(GlassOpacityProperty, value);
        }

        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public CornerRadius BorderRadius
        {
            get => (CornerRadius)GetValue(BorderRadiusProperty);
            set => SetValue(BorderRadiusProperty, value);
        }

        public Brush GlassBackground
        {
            get => (Brush)GetValue(GlassBackgroundProperty);
            set => SetValue(GlassBackgroundProperty, value);
        }

        public GlassButtonType ButtonType
        {
            get => (GlassButtonType)GetValue(ButtonTypeProperty);
            set => SetValue(ButtonTypeProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InitializeAnimations();
            UpdateGlassEffect();
        }

        private void InitializeAnimations()
        {
            // Hover in animation
            _hoverInStoryboard = new Storyboard();
            var scaleXAnimation = new DoubleAnimation
            {
                To = 1.02,
                Duration = TimeSpan.FromMilliseconds(150),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            var scaleYAnimation = new DoubleAnimation
            {
                To = 1.02,
                Duration = TimeSpan.FromMilliseconds(150),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            
            Storyboard.SetTarget(scaleXAnimation, this);
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
            Storyboard.SetTarget(scaleYAnimation, this);
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
            
            _hoverInStoryboard.Children.Add(scaleXAnimation);
            _hoverInStoryboard.Children.Add(scaleYAnimation);

            // Hover out animation
            _hoverOutStoryboard = new Storyboard();
            var scaleXOutAnimation = new DoubleAnimation
            {
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(150),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            var scaleYOutAnimation = new DoubleAnimation
            {
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(150),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            
            Storyboard.SetTarget(scaleXOutAnimation, this);
            Storyboard.SetTargetProperty(scaleXOutAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
            Storyboard.SetTarget(scaleYOutAnimation, this);
            Storyboard.SetTargetProperty(scaleYOutAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
            
            _hoverOutStoryboard.Children.Add(scaleXOutAnimation);
            _hoverOutStoryboard.Children.Add(scaleYOutAnimation);

            // Press animation
            _pressStoryboard = new Storyboard();
            var pressScaleXAnimation = new DoubleAnimation
            {
                To = 0.98,
                Duration = TimeSpan.FromMilliseconds(100),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            var pressScaleYAnimation = new DoubleAnimation
            {
                To = 0.98,
                Duration = TimeSpan.FromMilliseconds(100),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            
            Storyboard.SetTarget(pressScaleXAnimation, this);
            Storyboard.SetTargetProperty(pressScaleXAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
            Storyboard.SetTarget(pressScaleYAnimation, this);
            Storyboard.SetTargetProperty(pressScaleYAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
            
            _pressStoryboard.Children.Add(pressScaleXAnimation);
            _pressStoryboard.Children.Add(pressScaleYAnimation);

            // Set up transform if not already set
            if (RenderTransform == null || RenderTransform is MatrixTransform)
            {
                RenderTransform = new ScaleTransform(1, 1);
                RenderTransformOrigin = new Point(0.5, 0.5);
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            _hoverInStoryboard?.Begin();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            _hoverOutStoryboard?.Begin();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            _pressStoryboard?.Begin();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (IsMouseOver)
            {
                _hoverInStoryboard?.Begin();
            }
            else
            {
                _hoverOutStoryboard?.Begin();
            }
        }

        private void UpdateGlassEffect()
        {
            if (GlassBackground == null)
            {
                var defaultBrush = ButtonType switch
                {
                    GlassButtonType.Primary => TryFindResource("Purple500Brush") as Brush,
                    GlassButtonType.Secondary => TryFindResource("GlassGradientBrush") as Brush,
                    GlassButtonType.Success => TryFindResource("SuccessBrush") as Brush,
                    GlassButtonType.Warning => TryFindResource("WarningBrush") as Brush,
                    GlassButtonType.Error => TryFindResource("ErrorBrush") as Brush,
                    _ => TryFindResource("GlassGradientBrush") as Brush
                };
                
                if (defaultBrush != null)
                {
                    GlassBackground = defaultBrush;
                }
            }

            // Apply shadow effect
            var shadowEffect = new DropShadowEffect
            {
                Color = Colors.Black,
                Direction = 270,
                ShadowDepth = 2,
                BlurRadius = 8,
                Opacity = 0.15
            };
            Effect = shadowEffect;
        }
    }

    public enum GlassButtonType
    {
        Default,
        Primary,
        Secondary,
        Success,
        Warning,
        Error
    }
}