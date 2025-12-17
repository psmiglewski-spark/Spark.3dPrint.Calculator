    namespace Spark._3dPrint.Calculator
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);

            window.Created += (s, e) =>
            {
                // Log startup
                System.Diagnostics.Debug.WriteLine("Application started successfully");
            };

            return window;
        }
    }
}
