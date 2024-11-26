using System.Windows;
using System.Windows.Input;
using CelltechFinal.ViewModels;

namespace CelltechFinal.Views.Ventas
{
    public partial class NuevaVentaWindow : Window
    {
        public NuevaVentaWindow()
        {
            InitializeComponent();
            DataContext = new NuevaVentaViewModel();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void TxtCodigoProducto_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is NuevaVentaViewModel viewModel)
                {
                    viewModel.BuscarProductoCommand.Execute(null);
                }
            }
        }
    }
}