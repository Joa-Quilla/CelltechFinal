using System.Windows;
using CelltechFinal.ViewModels.Inventario;

namespace CelltechFinal.Views.InventarioProductos
{
    public partial class MarcasView : Window
    {
        public MarcasView()
        {
            InitializeComponent();
            DataContext = new MarcasViewModel();
        }
    }
}