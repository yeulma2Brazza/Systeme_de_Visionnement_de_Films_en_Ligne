using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;
using KasomaFlix.Application.UseCases.GestionProfil;
using KasomaFlix.Application.UseCases.GestionAbonnements;
using KasomaFlix.Presentation;
using KasomaFlix.Presentation.Services;

namespace KasomaFlix.Presentation.Views
{
    public partial class MonCompte : Page
    {
        public MonCompte()
        {
            InitializeComponent();
            Loaded += MonCompte_Loaded;
        }

        private async void MonCompte_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UserSession.IsLoggedIn() || !UserSession.GetUserId().HasValue)
            {
                NavigationService.Navigate(new FormulaireConnexion());
                return;
            }

            await ChargerInformationsAsync();
        }

        private async Task ChargerInformationsAsync()
        {
            try
            {
                var userId = UserSession.GetUserId();
                if (!userId.HasValue)
                {
                    return;
                }

                // Créer un scope pour isoler cette opération
                using (var scope = ServiceLocator.CreateScope())
                {
                    var obtenirProfilUseCase = scope.ServiceProvider.GetRequiredService<ObtenirProfilUseCase>();
                    var obtenirAbonnementsUseCase = scope.ServiceProvider.GetRequiredService<ObtenirAbonnementsUseCase>();

                    var profil = await obtenirProfilUseCase.ExecuteAsync(userId.Value);

                    if (profil != null)
                    {
                        TxtNomComplet.Text = $"{profil.Prenom} {profil.Nom}";
                        TxtCourriel.Text = profil.Courriel;
                        TxtSolde.Text = $"{profil.Solde:F2} $";

                        // Charger les abonnements
                        var abonnements = await obtenirAbonnementsUseCase.ExecuteAsync(userId.Value);
                        var abonnementActif = abonnements.FirstOrDefault(a => a.EstActif);
                        if (abonnementActif != null)
                        {
                            TxtStatutAbonnement.Text = $"{abonnementActif.TypeAbonnement} Actif (Jusqu'au {abonnementActif.DateFin:dd/MM/yyyy})";
                            TxtStatutAbonnement.Foreground = System.Windows.Media.Brushes.LightGreen;
                        }
                        else
                        {
                            TxtStatutAbonnement.Text = "Aucun abonnement actif";
                            TxtStatutAbonnement.Foreground = System.Windows.Media.Brushes.Orange;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des informations : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Accueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AccueilCatalogue());
        }

        private void Deconnexion_Click(object sender, RoutedEventArgs e)
        {
            UserSession.Logout();
            NavigationService.Navigate(new FormulaireConnexion());
        }

        private void Profil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ModificationProfil());
        }

        private void ModifierProfil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ModificationProfil());
        }

        private void Abonnements_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageAbonnement());
        }

        private void Historique_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HistoriqueTransactions());
        }

        private void SoldePaiement_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SoldePaiement());
        }
    }
}
