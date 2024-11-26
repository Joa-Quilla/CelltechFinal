using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using CelltechFinal.Services;
using CelltechFinal.Views.Login;
using CelltechFinal.Views.Menu;

namespace CelltechFinal.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly UsuarioService _usuarioService;
        private string _username;
        private string _password;
        private string _errorMessage;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public ICommand LoginCommand { get; private set; }

        public LoginViewModel()
        {
            _usuarioService = new UsuarioService();
            LoginCommand = new RelayCommand(param => ExecuteLogin());
        }

        private void ExecuteLogin()
        {
            try
            {
                var usuario = _usuarioService.ValidateUser(Username, Password);
                if (usuario != null)
                {
                    var menuWindow = new MenuPrincipal();
                    menuWindow.Show();

                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is LoginWindow)
                        {
                            window.Close();
                            break;
                        }
                    }
                }
                else
                {
                    ErrorMessage = "Usuario o contraseña incorrectos";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error al intentar iniciar sesión";
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    // Implementación básica de ICommand
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}