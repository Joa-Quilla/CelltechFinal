using CelltechFinal.Data.Connection;
using CelltechFinal.Models;
using CelltechFinal.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using System.Linq;

namespace CelltechFinal.ViewModels
{
    public class NuevaVentaViewModel : ViewModelBase
    {
        private readonly VentaService _ventaService;
        private readonly ProductoService _productoService;
        private readonly MetodoPagoService _metodoPagoService;

        private string _codigoProducto;
        private int _cantidad = 1;
        private ObservableCollection<MetodoPago> _metodosPago;
        private MetodoPago _metodoPagoSeleccionado;
        private decimal _total;
        private Producto _productoSeleccionado;

        public ObservableCollection<DetalleVenta> Detalles { get; } = new ObservableCollection<DetalleVenta>();

        public string CodigoProducto
        {
            get => _codigoProducto;
            set => SetProperty(ref _codigoProducto, value);
        }

        public int Cantidad
        {
            get => _cantidad;
            set => SetProperty(ref _cantidad, value);
        }

        public ObservableCollection<MetodoPago> MetodosPago
        {
            get => _metodosPago;
            set => SetProperty(ref _metodosPago, value);
        }

        public MetodoPago MetodoPagoSeleccionado
        {
            get => _metodoPagoSeleccionado;
            set => SetProperty(ref _metodoPagoSeleccionado, value);
        }

        public decimal Total
        {
            get => _total;
            set => SetProperty(ref _total, value);
        }

        public ICommand BuscarProductoCommand { get; private set; }
        public ICommand AgregarProductoCommand { get; private set; }
        public ICommand QuitarProductoCommand { get; private set; }
        public ICommand GuardarVentaCommand { get; private set; }

        public NuevaVentaViewModel()
        {
            _ventaService = new VentaService();
            _productoService = new ProductoService();
            _metodoPagoService = new MetodoPagoService();

            InitializeCommands();
            CargarMetodosPago();
        }

        private void InitializeCommands()
        {
            BuscarProductoCommand = new RelayCommand(o => BuscarProducto());
            AgregarProductoCommand = new RelayCommand(o => AgregarProducto(), o => PuedeAgregarProducto());
            QuitarProductoCommand = new RelayCommand(o => QuitarProducto((DetalleVenta)o));
            GuardarVentaCommand = new RelayCommand(o => GuardarVenta(), o => PuedeGuardarVenta());
        }

        private void CargarMetodosPago()
        {
            try
            {
                var metodos = _metodoPagoService.GetAllMetodosPago();
                MetodosPago = new ObservableCollection<MetodoPago>(metodos);
                MetodoPagoSeleccionado = MetodosPago.FirstOrDefault();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar métodos de pago: {ex.Message}");
            }
        }

        private void BuscarProducto()
        {
            try
            {
                _productoSeleccionado = _productoService.ObtenerPorCodigo(CodigoProducto);
                if (_productoSeleccionado == null)
                {
                    MessageBox.Show("Producto no encontrado");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar producto: {ex.Message}");
            }
        }

        private void AgregarProducto()
        {
            if (_productoSeleccionado == null || Cantidad <= 0) return;

            var detalle = new DetalleVenta
            {
                ProductoID = _productoSeleccionado.ProductoID,
                Codigo = _productoSeleccionado.Codigo,
                Nombre = _productoSeleccionado.Nombre,
                Cantidad = Cantidad,
                PrecioUnitario = _productoSeleccionado.PrecioVenta,
                Subtotal = Cantidad * _productoSeleccionado.PrecioVenta
            };

            Detalles.Add(detalle);
            ActualizarTotal();
            LimpiarProductoActual();
        }

        private void QuitarProducto(DetalleVenta detalle)
        {
            if (detalle != null)
            {
                Detalles.Remove(detalle);
                ActualizarTotal();
            }
        }

        private void GuardarVenta()
        {
            try
            {
                var venta = new Venta
                {
                    MetodoPagoID = MetodoPagoSeleccionado.MetodoPagoID,
                    Subtotal = Total,
                    Total = Total,
                    Detalles = Detalles.ToList()
                };

                _ventaService.GuardarVenta(venta);
                MessageBox.Show("Venta guardada correctamente");
                LimpiarVenta();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la venta: {ex.Message}");
            }
        }

        private void ActualizarTotal()
        {
            Total = Detalles.Sum(d => d.Subtotal);
        }

        private bool PuedeAgregarProducto()
        {
            return _productoSeleccionado != null && Cantidad > 0;
        }

        private bool PuedeGuardarVenta()
        {
            return Detalles.Any() && MetodoPagoSeleccionado != null;
        }

        private void LimpiarProductoActual()
        {
            CodigoProducto = string.Empty;
            Cantidad = 1;
            _productoSeleccionado = null;
        }

        private void LimpiarVenta()
        {
            Detalles.Clear();
            Total = 0;
            LimpiarProductoActual();
        }
    }
}