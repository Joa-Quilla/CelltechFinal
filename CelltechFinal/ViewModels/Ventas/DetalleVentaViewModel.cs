using CelltechFinal.Data.Connection;

namespace CelltechFinal.ViewModels.Ventas
{
    public class DetalleVentaViewModel : ViewModelBase
    {
        private int _productoID;
        private string _codigo;
        private string _nombre;
        private int _cantidad;
        private decimal _precioUnitario;

        public int ProductoID
        {
            get => _productoID;
            set => SetProperty(ref _productoID, value);
        }

        public string Codigo
        {
            get => _codigo;
            set => SetProperty(ref _codigo, value);
        }

        public string Nombre
        {
            get => _nombre;
            set => SetProperty(ref _nombre, value);
        }

        public int Cantidad
        {
            get => _cantidad;
            set
            {
                if (SetProperty(ref _cantidad, value))
                {
                    OnPropertyChanged(nameof(Subtotal));
                }
            }
        }

        public decimal PrecioUnitario
        {
            get => _precioUnitario;
            set
            {
                if (SetProperty(ref _precioUnitario, value))
                {
                    OnPropertyChanged(nameof(Subtotal));
                }
            }
        }

        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}