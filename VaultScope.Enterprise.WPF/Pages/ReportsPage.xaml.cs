using System.Windows.Controls;
using VaultScope.Enterprise.WPF.ViewModels;

namespace VaultScope.Enterprise.WPF.Pages;

/// <summary>
/// Interaction logic for ReportsPage.xaml
/// </summary>
public partial class ReportsPage : UserControl
{
    private readonly ReportsViewModel _viewModel;

    public ReportsPage()
    {
        InitializeComponent();
        _viewModel = new ReportsViewModel();
        DataContext = _viewModel;
    }
}