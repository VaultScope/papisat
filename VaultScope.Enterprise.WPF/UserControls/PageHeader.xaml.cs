using System.Windows;
using System.Windows.Controls;

namespace VaultScope.Enterprise.WPF.UserControls;

/// <summary>
/// Interaction logic for PageHeader.xaml
/// </summary>
public partial class PageHeader : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(PageHeader), new PropertyMetadata("Page Title", OnTitleChanged));

    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register("Subtitle", typeof(string), typeof(PageHeader), new PropertyMetadata("Page subtitle", OnSubtitleChanged));

    public static readonly DependencyProperty BreadcrumbProperty =
        DependencyProperty.Register("Breadcrumb", typeof(string), typeof(PageHeader), new PropertyMetadata("Current Page", OnBreadcrumbChanged));

    public static readonly DependencyProperty PrimaryActionTextProperty =
        DependencyProperty.Register("PrimaryActionText", typeof(string), typeof(PageHeader), new PropertyMetadata("", OnPrimaryActionTextChanged));

    public static readonly DependencyProperty SecondaryActionTextProperty =
        DependencyProperty.Register("SecondaryActionText", typeof(string), typeof(PageHeader), new PropertyMetadata("", OnSecondaryActionTextChanged));

    public static readonly DependencyProperty ShowPrimaryActionProperty =
        DependencyProperty.Register("ShowPrimaryAction", typeof(bool), typeof(PageHeader), new PropertyMetadata(false, OnShowPrimaryActionChanged));

    public static readonly DependencyProperty ShowSecondaryActionProperty =
        DependencyProperty.Register("ShowSecondaryAction", typeof(bool), typeof(PageHeader), new PropertyMetadata(false, OnShowSecondaryActionChanged));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public string Breadcrumb
    {
        get => (string)GetValue(BreadcrumbProperty);
        set => SetValue(BreadcrumbProperty, value);
    }

    public string PrimaryActionText
    {
        get => (string)GetValue(PrimaryActionTextProperty);
        set => SetValue(PrimaryActionTextProperty, value);
    }

    public string SecondaryActionText
    {
        get => (string)GetValue(SecondaryActionTextProperty);
        set => SetValue(SecondaryActionTextProperty, value);
    }

    public bool ShowPrimaryAction
    {
        get => (bool)GetValue(ShowPrimaryActionProperty);
        set => SetValue(ShowPrimaryActionProperty, value);
    }

    public bool ShowSecondaryAction
    {
        get => (bool)GetValue(ShowSecondaryActionProperty);
        set => SetValue(ShowSecondaryActionProperty, value);
    }

    public event EventHandler<RoutedEventArgs>? PrimaryActionClicked;
    public event EventHandler<RoutedEventArgs>? SecondaryActionClicked;

    public PageHeader()
    {
        InitializeComponent();
        
        PrimaryActionButton.Click += (s, e) => PrimaryActionClicked?.Invoke(this, e);
        SecondaryActionButton.Click += (s, e) => SecondaryActionClicked?.Invoke(this, e);
    }

    private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PageHeader header)
        {
            header.PageTitle.Text = e.NewValue?.ToString() ?? "";
        }
    }

    private static void OnSubtitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PageHeader header)
        {
            header.PageSubtitle.Text = e.NewValue?.ToString() ?? "";
        }
    }

    private static void OnBreadcrumbChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PageHeader header)
        {
            header.BreadcrumbCurrent.Text = e.NewValue?.ToString() ?? "";
        }
    }

    private static void OnPrimaryActionTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PageHeader header)
        {
            header.PrimaryActionButton.Content = e.NewValue?.ToString() ?? "";
        }
    }

    private static void OnSecondaryActionTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PageHeader header)
        {
            header.SecondaryActionButton.Content = e.NewValue?.ToString() ?? "";
        }
    }

    private static void OnShowPrimaryActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PageHeader header)
        {
            header.PrimaryActionButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private static void OnShowSecondaryActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PageHeader header)
        {
            header.SecondaryActionButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}