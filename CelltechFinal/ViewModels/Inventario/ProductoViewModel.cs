using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CelltechFinal.Models;
using CelltechFinal.Services;
using CelltechFinal.Data.Connection;

namespace CelltechFinal.ViewModels.Inventario
{
    public class ProductoViewModel : ViewModelBase
    {
        private readonly ProductoService _productoService;
        private readonly MarcaService _marcaService;
        private readonly CategoriaService _categoriaService;

        private ObservableCollection<Marca> _marcas;
        private ObservableCollection<Categoria> _categorias;
        private Producto _productoOriginal;
        private bool _esEdicion;

        private string _codigo;
        private string _nombre;
        private string _descripcion;
        private int _categoriaID;
        private int _marcaID;
        private string _modelo;
        private decimal _precioVenta;
        private int _stock;
        private int _stockMinimo;
        private bool _estado = true;

        private bool _isScanning;
        private string _ultimoCodigoEscaneado;
        private DateTime _ultimoEscaneo;
        private const int TIEMPO_ENTRE_ESCANEOS = 1000;

        #region Propiedades Públicas

        public ObservableCollection<Marca> Marcas
        {
            get => _marcas;
            set => SetProperty(ref _marcas, value);
        }

        public ObservableCollection<Categoria> Categorias
        {
            get => _categorias;
            set => SetProperty(ref _categorias, value);
        }

        public string Codigo
        {
            get => _codigo;
            set
            {
                if (SetProperty(ref _codigo, value) && _isScanning)
                {
                    ValidarCodigoExistente(value);
                }
            }
        }

        public string Nombre
        {
            get => _nombre;
            set => SetProperty(ref _nombre, value);
        }

        public string Descripcion
        {
            get => _descripcion;
            set => SetProperty(ref _descripcion, value);
        }

        public int CategoriaID
        {
            get => _categoriaID;
            set => SetProperty(ref _categoriaID, value);
        }

        public int MarcaID
        {
            get => _marcaID;
            set => SetProperty(ref _marcaID, value);
        }

        public string Modelo
        {
            get => _modelo;
            set => SetProperty(ref _modelo, value);
        }

        public decimal PrecioVenta
        {
            get => _precioVenta;
            set => SetProperty(ref _precioVenta, value);
        }

        public int Stock
        {
            get => _stock;
            set => SetProperty(ref _stock, value);
        }

        public int StockMinimo
        {
            get => _stockMinimo;
            set => SetProperty(ref _stockMinimo, value);
        }

        public bool Estado
        {
            get => _estado;
            set => SetProperty(ref _estado, value);
        }

        #endregion

        #region Comandos

        public ICommand GuardarCommand { get; private set; }
        public ICommand CancelarCommand { get; private set; }
        public ICommand IncrementarStockCommand { get; private set; }
        public ICommand DecrementarStockCommand { get; private set; }
        public ICommand IncrementarStockMinimoCommand { get; private set; }
        public ICommand DecrementarStockMinimoCommand { get; private set; }
        public ICommand ActivarEscanerCommand { get; private set; }

        #endregion

        public ProductoViewModel()
        {
            _productoService = new ProductoService();
            _marcaService = new MarcaService();
            _categoriaService = new CategoriaService();

            InitializeCommands();
            CargarDatos();
        }

        private void InitializeCommands()
        {
            GuardarCommand = new RelayCommand(o => GuardarProducto());
            CancelarCommand = new RelayCommand(o => CerrarVentana());
            IncrementarStockCommand = new RelayCommand(o => Stock++);
            DecrementarStockCommand = new RelayCommand(o => { if (Stock > 0) Stock--; });
            IncrementarStockMinimoCommand = new RelayCommand(o => StockMinimo++);
            DecrementarStockMinimoCommand = new RelayCommand(o => { if (StockMinimo > 0) StockMinimo--; });
            ActivarEscanerCommand = new RelayCommand(o => ActivarEscaner());
        }

        private void CargarDatos()
        {
            try
            {
                var marcas = _marcaService.GetAllMarcas();
                Marcas = new ObservableCollection<Marca>(marcas);

                var categorias = _categoriaService.GetAllCategorias();
                Categorias = new ObservableCollection<Categoria>(categorias);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CargarProductoParaEditar(Producto producto)
        {
            _productoOriginal = producto;
            _esEdicion = true;

            Codigo = producto.Codigo;
            Nombre = producto.Nombre;
            Descripcion = producto.Descripcion;
            CategoriaID = producto.CategoriaID;
            MarcaID = producto.MarcaID;
            Modelo = producto.Modelo;
            PrecioVenta = producto.PrecioVenta;
            Stock = producto.Stock;
            StockMinimo = producto.StockMinimo;
            Estado = producto.Estado;
        }

        private void GuardarProducto()
        {
            try
            {
                if (!ValidarFormulario()) return;

                // Primero verificamos si el código ya existe
                var productosExistentes = _productoService.BuscarProductos(Codigo.Trim());
                if (productosExistentes.Count > 0 && (!_esEdicion || productosExistentes[0].ProductoID != _productoOriginal?.ProductoID))
                {
                    MessageBox.Show("Ya existe un producto con este código. Por favor, use un código diferente.",
                        "Código duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var producto = new Producto
                {
                    Codigo = Codigo.Trim(),
                    Nombre = Nombre.Trim(),
                    Descripcion = Descripcion?.Trim(),
                    CategoriaID = CategoriaID,
                    MarcaID = MarcaID,
                    Modelo = Modelo?.Trim(),
                    PrecioVenta = PrecioVenta,
                    Stock = Stock,
                    StockMinimo = StockMinimo,
                    Estado = Estado
                };

                if (_esEdicion)
                {
                    producto.ProductoID = _productoOriginal.ProductoID;
                    _productoService.ActualizarProducto(producto);
                    MessageBox.Show("Producto actualizado correctamente",
                        "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    CerrarVentana(); // Solo cerramos si es edición
                }
                else
                {
                    _productoService.CrearProducto(producto);
                    MessageBox.Show("Producto guardado correctamente",
                        "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    LimpiarFormulario(); // Limpiamos en lugar de cerrar
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al {(_esEdicion ? "actualizar" : "guardar")} producto: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LimpiarFormulario()
        {
            Codigo = string.Empty;
            Nombre = string.Empty;
            Descripcion = string.Empty;
            Modelo = string.Empty;
            PrecioVenta = 0;
            Stock = 0;
            StockMinimo = 0;
            CategoriaID = 0;
            MarcaID = 0;
            Estado = true;
            _esEdicion = false;
            _productoOriginal = null;
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(Codigo))
            {
                MessageBox.Show("El código es obligatorio",
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Nombre))
            {
                MessageBox.Show("El nombre es obligatorio",
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (CategoriaID == 0)
            {
                MessageBox.Show("Debe seleccionar una categoría",
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (MarcaID == 0)
            {
                MessageBox.Show("Debe seleccionar una marca",
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (PrecioVenta <= 0)
            {
                MessageBox.Show("El precio de venta debe ser mayor a 0",
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (Stock < 0)
            {
                MessageBox.Show("El stock no puede ser negativo",
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (StockMinimo < 0)
            {
                MessageBox.Show("El stock mínimo no puede ser negativo",
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void ValidarCodigoExistente(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo)) return;

            try
            {
                var productos = _productoService.BuscarProductos(codigo);
                if (productos.Count > 0 && (!_esEdicion || productos[0].ProductoID != _productoOriginal?.ProductoID))
                {
                    MessageBox.Show("Ya existe un producto con este código",
                        "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al validar código: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActivarEscaner()
        {
            try
            {
                _isScanning = !_isScanning;
                _ultimoCodigoEscaneado = null;
                _ultimoEscaneo = DateTime.MinValue;

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

        private void CerrarVentana()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}