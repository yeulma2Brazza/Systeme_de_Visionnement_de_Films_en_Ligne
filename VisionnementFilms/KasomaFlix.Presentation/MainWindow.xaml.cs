using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace KasomaFlix.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Navigation initiale vers la page d'accueil
            MainFrame.Navigate(new Views.AccueilCatalogue());
        }
    }
}