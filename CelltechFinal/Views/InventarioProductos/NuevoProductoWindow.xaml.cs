using System.Windows;
using CelltechFinal.ViewModels.Inventario;

namespace CelltechFinal.Views.InventarioProductos
{
    public partial class NuevoProductoWindow : Window
    {
        public NuevoProductoWindow()
        {
            InitializeComponent();
            DataContext = new ProductoViewModel();
        }
    }
}