using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CelltechFinal.ViewModels;

namespace CelltechFinal.Views.Ventas
{
    public partial class NuevaVentaWindow : Window

    {


        private bool isMaximized = false;
        private readonly TextBlock maximizeIcon;
        private const WindowState maximized = WindowState.Maximized;

        public NuevaVentaWindow()
        {
            InitializeComponent();
            DataContext = new NuevaVentaViewModel();
            // Iniciar maximizado
            WindowState = maximized;


            // Inicializar el ícono de maximizar
            maximizeIcon = FindName("MaximizeIcon") as TextBlock;
            if (maximizeIcon != null)
            {
                maximizeIcon.Text = "❐";
            }
        }

        private void CenterWindow()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            double left = (screenWidth - this.Width) / 2;
            double top = (screenHeight - this.Height) / 2;

            this.Left = left;
            this.Top = top;
        }


        #region Botones de control de ventana
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (isMaximized)
            {
                // Restaurar
                WindowState = WindowState.Normal;
                Width = 1080; // Restaurar a 1080
                Height = 650; // Restaurar a 720
                CenterWindow();
                isMaximized = false;

                if (maximizeIcon != null)
                {
                    maximizeIcon.Text = "☐";
                }
            }
            else
            {
                // Maximizar
                WindowState = WindowState.Maximized;
                isMaximized = true;

                if (maximizeIcon != null)
                {
                    maximizeIcon.Text = "❐";
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        #endregion

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