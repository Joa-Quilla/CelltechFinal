using System.Windows;
using CelltechFinal.ViewModels;
using CelltechFinal.ViewModels.Inventario;
using CelltechFinal.Views.Menu;

namespace CelltechFinal.Views.InventarioProductos
{
    public partial class InventarioWindow : Window
    {
        public InventarioWindow()
        {
            InitializeComponent();
            DataContext = new InventarioViewModel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MenuPrincipal InventarioWindow = new MenuPrincipal();
            InventarioWindow.Show();
            this.Close();
        }
    }
}