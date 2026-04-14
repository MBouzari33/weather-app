using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using WeatherApp.Commands;


namespace WeatherApp.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // ========================
        // ENUM (Zustandsmodell)
        // ========================
        public enum AppState
        {
            Initial,
            Loading,
            Success,
            Error
        }

        // ========================
        // FIELDS
        // ========================
        private string _city = string.Empty;
        private double _temperature;
        private string _description = string.Empty;
        private string _errorMessage = string.Empty;
        private AppState _state = AppState.Initial;

        // ========================
        // PROPERTIES
        // ========================

        public string City
        {
            get => _city;
            set
            {
                if (_city == value) return;
                _city = value;
                OnPropertyChanged(nameof(City)); 

                // Wenn sich City ändert → Button evtl. neu bewerten
                SearchCommand.RaiseCanExecuteChanged();
            }
        }

        public double Temperature
        {
            get => _temperature;
            set
            {
                if (_temperature == value) return;
                _temperature = value;
                OnPropertyChanged(nameof(Temperature));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (_description == value) return;
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage == value) return;
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        // Remove the invalid line "aglay" from the State property setter
        public AppState State
        {
            get => _state;
            set
            {
                if (_state == value) return;
                _state = value;
                OnPropertyChanged(nameof(State));

                // WICHTIG:
                // Wenn State sich ändert → Button neu prüfen
                SearchCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsLoading => State == AppState.Loading;

        // ========================
        // COMMANDS
        // ========================

        public RelayCommand SearchCommand { get; }

        // ========================
        // CONSTRUCTOR
        // ========================

        public MainViewModel()
        {
            SearchCommand = new RelayCommand(
                execute: _ => Search(),
                canExecute: _ => CanSearch()
            );
        }

        // ========================
        // LOGIK
        // ========================

        private bool CanSearch()
        {
            return !string.IsNullOrWhiteSpace(City)
                   && State != AppState.Loading;
        }

        private async void Search()
        {
            try
            {
                State = AppState.Loading;
                ErrorMessage = string.Empty;

                // Simulierte API
                await Task.Delay(1500);

                Temperature = 22.5;
                Description = "Sunny";

                State = AppState.Success;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                State = AppState.Error;
            }
        }

        // ========================
        // INotifyPropertyChanged
        // ========================

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }
    }

}