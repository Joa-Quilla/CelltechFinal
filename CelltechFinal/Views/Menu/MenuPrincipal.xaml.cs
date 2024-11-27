using System.Windows;
using System.Windows.Controls;
using CelltechFinal.ViewModels;

namespace CelltechFinal.Views.Menu
{
    public partial class MenuPrincipal : Window
    {
        private bool isMaximized = false;
        private readonly TextBlock maximizeIcon;
        private const WindowState maximized = WindowState.Maximized;

        public MenuPrincipal()
        {
            InitializeComponent();
            DataContext = new MenuPrincipalViewModel();

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}