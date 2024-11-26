using System.Windows;
using CelltechFinal.ViewModels.Inventario;

namespace CelltechFinal.Views.InventarioProductos
{
    public partial class CategoriasView : Window
    {
        public CategoriasView()
        {
            InitializeComponent();
            DataContext = new CategoriasViewModel();
        }
    }
}