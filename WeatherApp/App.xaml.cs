using System;
using System.Windows;
using WeatherApp.Services;

namespace WeatherApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var apiKey = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                MessageBox.Show("API Key missing.");
                Shutdown();
                return;
            }

            var service = new WeatherService();
            var viewModel = new WeatherApp.ViewModel.ViewModel(service);

            var mainWindow = new MainWindow
            {
                DataContext = viewModel
            };

            mainWindow.Show();
            
        }
    }
}