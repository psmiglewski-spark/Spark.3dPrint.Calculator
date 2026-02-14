using Spark._3dPrint.Calculator.ViewModels;

namespace Spark._3dPrint.Calculator.Views
{
    public partial class ArchivePage : ContentPage
    {
        public ArchivePage(ArchiveViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
