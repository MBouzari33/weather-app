using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WeatherApp.Infrastructure
{
    /// <summary>
    /// Async equivalent of a normal RelayCommand:
    /// - RelayCommand executes synchronous Action logic.
    /// - AsyncRelayCommand executes asynchronous Task logic via ExecuteAsync.
    /// - AsyncRelayCommand prevents re-entrancy while running and auto-raises CanExecuteChanged.
    /// </summary>
    public sealed class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _executeAsync;
        private readonly Func<bool> _canExecute;
        private bool _isExecuting;

        public AsyncRelayCommand(Func<Task> executeAsync, Func<bool> canExecute = null, Func<Task> execute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke() ?? true);
        }

        public async void Execute(object parameter) 
        {
            await ExecuteAsync(_executeAsync);
        }

        public async Task ExecuteAsync(Func<Task> executeAsync)
        {
            if (executeAsync == null) throw new ArgumentNullException(nameof(executeAsync));
            if (!CanExecute(null)) return;

            _isExecuting = true;
            RaiseCanExecuteChanged();

            try
            {
                await executeAsync();
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
