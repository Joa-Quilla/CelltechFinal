using CelltechFinal.Views.InventarioProductos;
using CelltechFinal.Views.Login;
using CelltechFinal.Views.Menu;  // Para MenuPrincipal
using System.ComponentModel;  // Para INotifyPropertyChanged
using System.Windows;
using System.Windows.Input;



namespace CelltechFinal.ViewModels

{
    public class MenuPrincipalViewModel : INotifyPropertyChanged
    {
        private string _nombreUsuario;

        public string NombreUsuario
        {
            get => _nombreUsuario;
            set
            {
                _nombreUsuario = value;
                OnPropertyChanged(nameof(NombreUsuario));
            }
        }

        public ICommand AbrirInventarioCommand { get; private set; }
        public ICommand AbrirVentasCommand { get; private set; }
        public ICommand CerrarSesionCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MenuPrincipalViewModel()
        {
            NombreUsuario = "Usuario: Auder";

            AbrirInventarioCommand = new RelayCommand(param => AbrirInventario());
            AbrirVentasCommand = new RelayCommand(param => AbrirVentas());
            CerrarSesionCommand = new RelayCommand(param => CerrarSesion());
        }

        private void AbrirInventario()
        {
            var inventarioWindow = new InventarioWindow();
            inventarioWindow.Show();

            // Cerrar la ventana del menú
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MenuPrincipal)
                {
                    window.Close();
                    break;
                }
            }
        }

        private void AbrirVentas()
        {
            var ventasWindow = new Views.Ventas.VentasWindow();
            // Asegúrate de que esta directiva using esté presente

            // Resto del código permanece igual
            ventasWindow.Show();

            // Cerrar la ventana del menú
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MenuPrincipal)
                {
                    window.Close();
                    break;
                }
            }
        }

        private void CerrarSesion()
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();

            // Cerrar la ventana actual del menú
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MenuPrincipal)
                {
                    window.Close();
                    break;
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}