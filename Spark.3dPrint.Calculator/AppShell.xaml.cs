namespace Spark._3dPrint.Calculator
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("SettingsPage", typeof(Views.SettingsPage));
        }
    }
}
