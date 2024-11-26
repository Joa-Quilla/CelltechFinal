using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CelltechFinal.Models;
using CelltechFinal.Services;
using CelltechFinal.Data.Connection;

namespace CelltechFinal.ViewModels.Inventario
{
    public class CategoriasViewModel : ViewModelBase
    {
        private readonly CategoriaService _categoriaService;
        private ObservableCollection<Categoria> _categorias;
        private string _nombreCategoria;
        private string _descripcionCategoria;
        private Categoria _categoriaSeleccionada;
        private bool _isEditing;

        public ObservableCollection<Categoria> Categorias
        {
            get => _categorias;
            set => SetProperty(ref _categorias, value);
        }

        public string NombreCategoria
        {
            get => _nombreCategoria;
            set => SetProperty(ref _nombreCategoria, value);
        }

        public string DescripcionCategoria
        {
            get => _descripcionCategoria;
            set => SetProperty(ref _descripcionCategoria, value);
        }

        public ICommand GuardarCommand { get; private set; }
        public ICommand EditarCommand { get; private set; }
        public ICommand EliminarCommand { get; private set; }

        public CategoriasViewModel()
        {
            _categoriaService = new CategoriaService();
            InitializeCommands();
            CargarCategorias();
        }

        private void InitializeCommands()
        {
            GuardarCommand = new RelayCommand(o => GuardarCategoria());
            EditarCommand = new RelayCommand(o => EditarCategoria((Categoria)o));
            EliminarCommand = new RelayCommand(o => EliminarCategoria((Categoria)o));
        }

        private void CargarCategorias()
        {
            try
            {
                var lista = _categoriaService.GetAllCategorias();
                Categorias = new ObservableCollection<Categoria>(lista);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar categorías: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LimpiarFormulario()
        {
            NombreCategoria = string.Empty;
            DescripcionCategoria = string.Empty;
            _categoriaSeleccionada = null;
            _isEditing = false;
        }

        private void GuardarCategoria()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NombreCategoria))
                {
                    MessageBox.Show("El nombre de la categoría es obligatorio", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_isEditing)
                {
                    _categoriaSeleccionada.Nombre = NombreCategoria;
                    _categoriaSeleccionada.Descripcion = DescripcionCategoria;
                    _categoriaService.ActualizarCategoria(_categoriaSeleccionada);
                    MessageBox.Show("Categoría actualizada correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var nuevaCategoria = new Categoria
                    {
                        Nombre = NombreCategoria,
                        Descripcion = DescripcionCategoria
                    };
                    _categoriaService.CrearCategoria(nuevaCategoria);
                    MessageBox.Show("Categoría guardada correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                CargarCategorias();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al {(_isEditing ? "actualizar" : "guardar")} categoría: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditarCategoria(Categoria categoria)
        {
            _categoriaSeleccionada = categoria;
            NombreCategoria = categoria.Nombre;
            DescripcionCategoria = categoria.Descripcion;
            _isEditing = true;
        }

        private void EliminarCategoria(Categoria categoria)
        {
            var resultado = MessageBox.Show($"¿Está seguro de eliminar la categoría {categoria.Nombre}?", 
                "Confirmar eliminación", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    _categoriaService.EliminarCategoria(categoria.CategoriaID);
                    CargarCategorias();
                    MessageBox.Show("Categoría eliminada correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar categoría: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}