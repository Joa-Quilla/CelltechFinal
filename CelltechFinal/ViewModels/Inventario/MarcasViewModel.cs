using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CelltechFinal.Models;
using CelltechFinal.Services;
using CelltechFinal.Data.Connection;

namespace CelltechFinal.ViewModels.Inventario
{
    public class MarcasViewModel : ViewModelBase
    {
        private readonly MarcaService _marcaService;
        private ObservableCollection<Marca> _marcas;
        private string _nombreMarca;
        private string _descripcionMarca;
        private Marca _marcaSeleccionada;
        private bool _isEditing;

        public ObservableCollection<Marca> Marcas
        {
            get => _marcas;
            set => SetProperty(ref _marcas, value);
        }

        public string NombreMarca
        {
            get => _nombreMarca;
            set => SetProperty(ref _nombreMarca, value);
        }

        public string DescripcionMarca
        {
            get => _descripcionMarca;
            set => SetProperty(ref _descripcionMarca, value);
        }

        public ICommand GuardarCommand { get; private set; }
        public ICommand EditarCommand { get; private set; }
        public ICommand EliminarCommand { get; private set; }

        public MarcasViewModel()
        {
            _marcaService = new MarcaService();
            InitializeCommands();
            CargarMarcas();
        }

        private void InitializeCommands()
        {
            GuardarCommand = new RelayCommand(o => GuardarMarca());
            EditarCommand = new RelayCommand(o => EditarMarca((Marca)o));
            EliminarCommand = new RelayCommand(o => EliminarMarca((Marca)o));
        }

        private void CargarMarcas()
        {
            try
            {
                var lista = _marcaService.GetAllMarcas();
                Marcas = new ObservableCollection<Marca>(lista);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar marcas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LimpiarFormulario()
        {
            NombreMarca = string.Empty;
            DescripcionMarca = string.Empty;
            _marcaSeleccionada = null;
            _isEditing = false;
        }

       private void GuardarMarca()
{
    try
    {
        if (string.IsNullOrWhiteSpace(NombreMarca))
        {
            MessageBox.Show("El nombre de la marca es obligatorio", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_isEditing)
        {
            _marcaSeleccionada.Nombre = NombreMarca;
            _marcaSeleccionada.Descripcion = DescripcionMarca;
            _marcaService.ActualizarMarca(_marcaSeleccionada);
            MessageBox.Show("Marca actualizada correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            var nuevaMarca = new Marca
            {
                Nombre = NombreMarca,
                Descripcion = DescripcionMarca
            };
            _marcaService.CrearMarca(nuevaMarca);
            MessageBox.Show("Marca guardada correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        CargarMarcas();
        LimpiarFormulario();
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error al {(_isEditing ? "actualizar" : "guardar")} marca: {ex.Message}", 
            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

        private void EditarMarca(Marca marca)
        {
            _marcaSeleccionada = marca;
            NombreMarca = marca.Nombre;
            DescripcionMarca = marca.Descripcion;
            _isEditing = true;
        }

        private void EliminarMarca(Marca marca)
        {
            var resultado = MessageBox.Show($"¿Está seguro de eliminar la marca {marca.Nombre}?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    _marcaService.EliminarMarca(marca.MarcaID);
                    CargarMarcas();
                    MessageBox.Show("Marca eliminada correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar marca: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}