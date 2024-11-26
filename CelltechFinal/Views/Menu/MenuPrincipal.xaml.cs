using System.Windows;
using CelltechFinal.ViewModels;

namespace CelltechFinal.Views.Menu
{
    public partial class MenuPrincipal : Window
    {
        public MenuPrincipal()
        {
            InitializeComponent();
            DataContext = new MenuPrincipalViewModel();
        }
    }
}