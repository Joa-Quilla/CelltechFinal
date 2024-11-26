using CelltechFinal.ViewModels.Ventas;
using System.Windows;

using CelltechFinal.ViewModels;
using CelltechFinal.Views.Menu;

namespace CelltechFinal.Views.Ventas
{
    public partial class VentasWindow : Window
    {
        public VentasWindow()
        {
            InitializeComponent();
            DataContext = new VentasViewModel();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
           MenuPrincipal VentasWindow = new MenuPrincipal();
            VentasWindow.Show();
            this.Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NuevaVentaWindow VentasWindow = new NuevaVentaWindow();
            VentasWindow.Show();
            this.Close();
        }
    }
}