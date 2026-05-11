using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;
using KasomaFlix.Application.UseCases.GestionAdmin;
using KasomaFlix.Presentation;
using KasomaFlix.Presentation.Services;

namespace KasomaFlix.Presentation.Views
{
    public partial class TableauBordAdmin : Page
    {
        public TableauBordAdmin()
        {
            InitializeComponent();
            Loaded += TableauBordAdmin_Loaded;
        }

        private async void TableauBordAdmin_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UserSession.IsLoggedIn() || !UserSession.IsAdmin())
            {
                NavigationService.Navigate(new FormulaireConnexion());
                return;
            }

            await ChargerStatistiquesAsync();
        }

        private async Task ChargerStatistiquesAsync()
        {
            try
            {
                // Créer un scope pour isoler cette opération
                using (var scope = ServiceLocator.CreateScope())
                {
                    var obtenirStatistiquesAdminUseCase = scope.ServiceProvider.GetRequiredService<ObtenirStatistiquesAdminUseCase>();
                    var statistiques = await obtenirStatistiquesAdminUseCase.ExecuteAsync();
                    TxtNombreMembres.Text = statistiques.NombreMembres.ToString("N0");
                    TxtNombreFilms.Text = statistiques.NombreFilms.ToString("N0");
                    TxtRevenusMois.Text = statistiques.RevenusMois.ToString("N2") + " $";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des statistiques : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Deconnexion_Click(object sender, RoutedEventArgs e)
        {
            UserSession.Logout();
            NavigationService.Navigate(new FormulaireConnexion());
        }

        private void GestionFilms_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GestionFilm());
        }

        private void GestionMembres_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GestionMembres());
        }

        private void GestionTransactions_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GestionTransactions());
        }

        private void GestionCategories_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("La gestion des catégories sera implémentée dans une version future.\n\nLes catégories sont actuellement gérées directement lors de l'ajout/modification de films.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
