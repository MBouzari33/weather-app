using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using WeatherApp.Infrastructure;
using WeatherApp.Services;

namespace WeatherApp.ViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        public enum AppState { Initial, Loading, Success, Error }

        private string _city = string.Empty;
        private double _temperature;
        private double _feelsLike;
        private int _humidity;
        private string _locationName = string.Empty;
        private string _condition = string.Empty;
        private string _description = string.Empty;
        private string _errorMessage = string.Empty;
        private AppState _state = AppState.Initial;
        private readonly IWeatherService _weatherService;

        public ViewModel(IWeatherService weatherService)
        {
            _weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
            SearchCommand = new AsyncRelayCommand(SearchAsync, CanSearch);
        }

        public string City
        {
            get => _city;
            set
            {
                if (_city == value) return;
                _city = value;
                OnPropertyChanged(nameof(City));
                SearchCommand.RaiseCanExecuteChanged();
            }
        }

        public double Temperature { get => _temperature; private set { _temperature = value; OnPropertyChanged(nameof(Temperature)); OnPropertyChanged(nameof(TemperatureDisplay)); } }
        public double FeelsLike { get => _feelsLike; private set { _feelsLike = value; OnPropertyChanged(nameof(FeelsLike)); OnPropertyChanged(nameof(FeelsLikeDisplay)); } }
        public int Humidity { get => _humidity; private set { _humidity = value; OnPropertyChanged(nameof(Humidity)); OnPropertyChanged(nameof(HumidityDisplay)); } }
        public string LocationName { get => _locationName; private set { _locationName = value; OnPropertyChanged(nameof(LocationName)); } }
        public string Condition { get => _condition; private set { _condition = value; OnPropertyChanged(nameof(Condition)); } }
        public string Description { get => _description; private set { _description = value; OnPropertyChanged(nameof(Description)); } }
        public string ErrorMessage { get => _errorMessage; private set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); } }

        public string TemperatureDisplay => string.Format(CultureInfo.CurrentCulture, "{0:0}°", Temperature);
        public string FeelsLikeDisplay => string.Format(CultureInfo.CurrentCulture, "{0:0}°C", FeelsLike);
        public string HumidityDisplay => string.Format(CultureInfo.CurrentCulture, "{0}%", Humidity);
        public bool IsLoading => State == AppState.Loading;
        public bool HasResult => State == AppState.Success;
        public bool HasError => State == AppState.Error;

        public AppState State
        {
            get => _state;
            private set
            {
                if (_state == value) return;
                _state = value;
                OnPropertyChanged(nameof(State));
                OnPropertyChanged(nameof(IsLoading));
                OnPropertyChanged(nameof(HasResult));
                OnPropertyChanged(nameof(HasError));
                SearchCommand.RaiseCanExecuteChanged();
            }
        }

        public AsyncRelayCommand SearchCommand { get; }

        private bool CanSearch() => !string.IsNullOrWhiteSpace(City) && State != AppState.Loading;

        private async Task SearchAsync()
        {
            try
            {
                State = AppState.Loading;
                ErrorMessage = string.Empty;
                var result = await _weatherService.GetWeatherAsync(City.Trim());
                if (result?.Main == null) throw new Exception("Invalid API response.");

                Temperature = result.Main.Temp;
                FeelsLike = result.Main.FeelsLike;
                Humidity = result.Main.Humidity;
                LocationName = string.IsNullOrWhiteSpace(result.Name) ? City.Trim() : result.Name;

                var weather = result.Weather != null && result.Weather.Count > 0 ? result.Weather[0] : null;
                Condition = weather?.Main ?? "Current weather";
                Description = ToSentenceCase(weather?.Description ?? "No description available");
                State = AppState.Success;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                State = AppState.Error;
            }
        }

        private static string ToSentenceCase(string value) =>
            string.IsNullOrWhiteSpace(value) ? value : char.ToUpper(value[0], CultureInfo.CurrentCulture) + value.Substring(1);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}