using System;
using System.ComponentModel;
using System.Threading.Tasks;
using WeatherApp.Services;
using WeatherApp.Infrastructure;

namespace WeatherApp.ViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        // =========================
        // STATE MODEL
        // =========================
        public enum AppState
        {
            Initial,
            Loading,
            Success,
            Error
        }

        // =========================
        // FIELDS
        // =========================
        private string _city = string.Empty;
        private double _temperature;
        private string _description = string.Empty;
        private string _errorMessage = string.Empty;
        private AppState _state = AppState.Initial;

        private readonly IWeatherService _weatherService;

        // =========================
        // PROPERTIES
        // =========================

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

        public double Temperature
        {
            get => _temperature;
            private set
            {
                if (_temperature == value) return;
                _temperature = value;
                OnPropertyChanged(nameof(Temperature));
            }
        }

        public string Description
        {
            get => _description;
            private set
            {
                if (_description == value) return;
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set
            {
                if (_errorMessage == value) return;
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public AppState State
        {
            get => _state;
            private set
            {
                if (_state == value) return;
                _state = value;

                OnPropertyChanged(nameof(State));
                OnPropertyChanged(nameof(IsLoading));

                SearchCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsLoading => State == AppState.Loading;

        // =========================
        // COMMANDS
        // =========================

        public AsyncRelayCommand SearchCommand { get; }

        // Replace the constructor assignment for SearchCommand with the correct number of arguments
        public ViewModel(IWeatherService weatherService)
        {
            _weatherService = weatherService;

            SearchCommand = new AsyncRelayCommand(
                executeAsync: SearchAsync,
                canExecute: CanSearch,
                execute: SearchAsync // Provide the third required argument as per the constructor signature
            );
        }

        // =========================
        // LOGIC
        // =========================

        private bool CanSearch()
        {
            return !string.IsNullOrWhiteSpace(City)
                   && State != AppState.Loading;
        }

        private async Task SearchAsync()
        {
            try
            {
                State = AppState.Loading;
                ErrorMessage = string.Empty;

                var result = await _weatherService.GetWeatherAsync(City);

                if (result?.Main == null)
                {
                    throw new Exception("Invalid API response.");
                }

                Temperature = result.Main.Temp;

                Description = result.Weather != null &&
                              result.Weather.Count > 0
                    ? result.Weather[0].Description
                    : "No description available";

                State = AppState.Success;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                State = AppState.Error;
            }
        }

        // =========================
        // INotifyPropertyChanged
        // =========================

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}