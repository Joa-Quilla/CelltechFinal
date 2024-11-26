using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CelltechFinal.Models;
using CelltechFinal.Services;
using CelltechFinal.Views.InventarioProductos;
using CelltechFinal.Data.Connection;
using CelltechFinal.ViewModels.Inventario;

namespace CelltechFinal.ViewModels.Inventario
{
    public class InventarioViewModel : ViewModelBase
    {
        private readonly ProductoService _productoService;
        private ObservableCollection<Producto> _productos;
        private string _textoBusqueda;
        private bool _isScanning;

        public ObservableCollection<Producto> Productos
        {
            get => _productos;
            set => SetProperty(ref _productos, value);
        }

        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                if (SetProperty(ref _textoBusqueda, value))
                {
                    if (_isScanning)
                    {
                        ProcesarCodigoEscaneado(value);
                    }
                    else
                    {
                        BuscarProductos(value);
                    }
                }
            }
        }

        public ICommand NuevoProductoCommand { get; private set; }
        public ICommand EditarProductoCommand { get; private set; }
        public ICommand EliminarProductoCommand { get; private set; }
        public ICommand GestionarCategoriasCommand { get; private set; }
        public ICommand GestionarMarcasCommand { get; private set; }
        public ICommand ActivarEscanerCommand { get; private set; }
        public ICommand LimpiarBusquedaCommand { get; private set; }

        public InventarioViewModel()
        {
            _productoService = new ProductoService();
            InitializeCommands();
            CargarProductos();
        }

        private void InitializeCommands()
        {
            NuevoProductoCommand = new RelayCommand(o => NuevoProducto());
            EditarProductoCommand = new RelayCommand(o => EditarProducto((Producto)o));
            EliminarProductoCommand = new RelayCommand(o => EliminarProducto((Producto)o));
            GestionarCategoriasCommand = new RelayCommand(o => GestionarCategorias());
            GestionarMarcasCommand = new RelayCommand(o => GestionarMarcas());
            ActivarEscanerCommand = new RelayCommand(o => ActivarEscaner());
            LimpiarBusquedaCommand = new RelayCommand(o => LimpiarBusqueda());
        }

        private void CargarProductos()
        {
            try
            {
                var productos = _productoService.GetAllProductos();
                Productos = new ObservableCollection<Producto>(productos);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productos: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NuevoProducto()
        {
            try
            {
                var nuevoProductoWindow = new NuevoProductoWindow();
                nuevoProductoWindow.ShowDialog();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir la ventana de nuevo producto: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditarProducto(Producto producto)
        {
            try
            {
                var nuevoProductoWindow = new NuevoProductoWindow();
                var viewModel = (ProductoViewModel)nuevoProductoWindow.DataContext;
                viewModel.CargarProductoParaEditar(producto);
                nuevoProductoWindow.ShowDialog();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al editar producto: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EliminarProducto(Producto producto)
        {
            try
            {
                var resultado = MessageBox.Show(
                    $"¿Está seguro de eliminar el producto {producto.Nombre}?",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultado == MessageBoxResult.Yes)
                {
                    _productoService.EliminarProducto(producto.ProductoID);
                    CargarProductos();
                    MessageBox.Show("Producto eliminado correctamente",
                        "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar producto: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GestionarCategorias()
        {
            try
            {
                var categoriasWindow = new CategoriasView();
                categoriasWindow.ShowDialog();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir gestión de categorías: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GestionarMarcas()
        {
            try
            {
                var marcasWindow = new MarcasView();
                marcasWindow.ShowDialog();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir gestión de marcas: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProcesarCodigoEscaneado(string codigo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codigo)) return;

                var productos = _productoService.BuscarProductos(codigo);
                if (productos.Count == 0)
                {
                    var respuesta = MessageBox.Show(
                        "No se encontró ningún producto con este código. ¿Desea crear uno nuevo?",
                        "Producto no encontrado",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (respuesta == MessageBoxResult.Yes)
                    {
                        var nuevoProductoWindow = new NuevoProductoWindow();
                        var viewModel = (ProductoViewModel)nuevoProductoWindow.DataContext;
                        viewModel.Codigo = codigo;
                        nuevoProductoWindow.ShowDialog();
                        CargarProductos();
                    }
                }
                else
                {
                    Productos = new ObservableCollection<Producto>(productos);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar código escaneado: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BuscarProductos(string busqueda)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(busqueda))
                {
                    CargarProductos();
                    return;
                }

                var productos = _productoService.BuscarProductos(busqueda);
                Productos = new ObservableCollection<Producto>(productos);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar productos: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActivarEscaner()
        {
            try
            {
                _isScanning = !_isScanning;
                TextoBusqueda = string.Empty;

                MessageBox.Show(_isScanning ?
                    "Escáner activado. Escanee el código de barras." :
                    "Escáner desactivado.",
                    "Escáner", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al manejar el escáner: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _isScanning = false;
            }
        }

        private void LimpiarBusqueda()
        {
            TextoBusqueda = string.Empty;
            CargarProductos();
        }
    }
}