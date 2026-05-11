using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;
using KasomaFlix.Application.UseCases.GestionAdmin;
using KasomaFlix.Presentation;
using KasomaFlix.Presentation.Services;

namespace KasomaFlix.Presentation.Views
{
    public partial class GestionMembres : Page
    {
        public GestionMembres()
        {
            InitializeComponent();
            Loaded += GestionMembres_Loaded;
        }

        private async void GestionMembres_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UserSession.IsLoggedIn() || !UserSession.IsAdmin())
            {
                NavigationService.Navigate(new FormulaireConnexion());
                return;
            }

            await ChargerMembresAsync();
        }

        private async Task ChargerMembresAsync()
        {
            try
            {
                // Créer un scope pour isoler cette opération
                using (var scope = ServiceLocator.CreateScope())
                {
                    var obtenirTousMembresUseCase = scope.ServiceProvider.GetRequiredService<ObtenirTousMembresUseCase>();
                    var membres = await obtenirTousMembresUseCase.ExecuteAsync();
                    DgMembres.ItemsSource = membres;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des membres : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TableauBord_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TableauBordAdmin());
        }

        private void Deconnexion_Click(object sender, RoutedEventArgs e)
        {
            UserSession.Logout();
            NavigationService.Navigate(new FormulaireConnexion());
        }
    }
}
