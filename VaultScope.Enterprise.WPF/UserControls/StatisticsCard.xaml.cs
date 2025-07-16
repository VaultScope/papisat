using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VaultScope.Enterprise.WPF.UserControls;

/// <summary>
/// Interaction logic for StatisticsCard.xaml
/// </summary>
public partial class StatisticsCard : UserControl
{
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register("Icon", typeof(string), typeof(StatisticsCard), new PropertyMetadata("&#xE9F9;", OnIconChanged));

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(StatisticsCard), new PropertyMetadata("Total Scans", OnTitleChanged));

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(string), typeof(StatisticsCard), new PropertyMetadata("0", OnValueChanged));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register("Description", typeof(string), typeof(StatisticsCard), new PropertyMetadata("security scans", OnDescriptionChanged));

    public static readonly DependencyProperty IconColorProperty =
        DependencyProperty.Register("IconColor", typeof(Brush), typeof(StatisticsCard), new PropertyMetadata(null, OnIconColorChanged));

    public static readonly DependencyProperty ValueColorProperty =
        DependencyProperty.Register("ValueColor", typeof(Brush), typeof(StatisticsCard), new PropertyMetadata(null, OnValueColorChanged));

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public Brush IconColor
    {
        get => (Brush)GetValue(IconColorProperty);
        set => SetValue(IconColorProperty, value);
    }

    public Brush ValueColor
    {
        get => (Brush)GetValue(ValueColorProperty);
        set => SetValue(ValueColorProperty, value);
    }

    public StatisticsCard()
    {
        InitializeComponent();
        
        // Set default colors
        if (IconColor == null)
        {
            IconColor = TryFindResource("InfoBlueBrush") as Brush;
        }
        
        if (ValueColor == null)
        {
            ValueColor = TryFindResource("TextPrimaryBrush") as Brush;
        }
    }

    private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is StatisticsCard card)
        {
            card.CardIcon.Text = e.NewValue?.ToString() ?? "";
        }
    }

    private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is StatisticsCard card)
        {
            card.CardTitle.Text = e.NewValue?.ToString() ?? "";
        }
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is StatisticsCard card)
        {
            card.CardValue.Text = e.NewValue?.ToString() ?? "";
        }
    }

    private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is StatisticsCard card)
        {
            card.CardDescription.Text = e.NewValue?.ToString() ?? "";
        }
    }

    private static void OnIconColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is StatisticsCard card && e.NewValue is Brush brush)
        {
            card.CardIcon.Foreground = brush;
        }
    }

    private static void OnValueColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is StatisticsCard card && e.NewValue is Brush brush)
        {
            card.CardValue.Foreground = brush;
        }
    }
}