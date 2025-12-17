using Spark._3dPrint.Calculator.ViewModels;

namespace Spark._3dPrint.Calculator.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage(SettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}