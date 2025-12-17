using Spark._3dPrint.Calculator.ViewModels;

namespace Spark._3dPrint.Calculator
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}