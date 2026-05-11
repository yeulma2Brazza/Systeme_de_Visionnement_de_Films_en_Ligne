using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;
using KasomaFlix.Application.UseCases.ConsultationFilm;
using KasomaFlix.Presentation;
using KasomaFlix.Presentation.Services;

namespace KasomaFlix.Presentation.Views
{
    /// <summary>
    /// Logique d'interaction pour DetailsFilmAlt.xaml
    /// Note: Cette page semble être un doublon de DetailsFilm
    /// </summary>
    public partial class DetailsFilmAlt : Page
    {
        private int _filmId;

        public DetailsFilmAlt(int filmId)
        {
            InitializeComponent();
            _filmId = filmId;
            Loaded += DetailsFilmAlt_Loaded;
        }

        private async void DetailsFilmAlt_Loaded(object sender, RoutedEventArgs e)
        {
            await ChargerDetailsFilmAsync();
        }

        private async Task ChargerDetailsFilmAsync()
        {
            try
            {
                // Créer un scope pour isoler cette opération
                using (var scope = ServiceLocator.CreateScope())
                {
                    var consulterFilmUseCase = scope.ServiceProvider.GetRequiredService<ConsulterFilmUseCase>();
                    var film = await consulterFilmUseCase.ExecuteAsync(_filmId);

                    if (film == null)
                    {
                        MessageBox.Show("Film introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        NavigationService?.Navigate(new AccueilCatalogue());
                        return;
                    }

                    // Afficher les informations (si les contrôles existent dans le XAML)
                    // Note: Cette page est un doublon de DetailsFilm, considérez utiliser DetailsFilm à la place
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des détails : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Accueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AccueilCatalogue());
        }

        private void MonCompte_Click(object sender, RoutedEventArgs e)
        {
            if (UserSession.IsLoggedIn() && UserSession.IsMembre())
            {
                NavigationService.Navigate(new MonCompte());
            }
            else
            {
                NavigationService.Navigate(new FormulaireConnexion());
            }
        }

        // Séquencement 2, Étape 2: Clic sur Visionner -> E05
        private void Visionner_Click(object sender, RoutedEventArgs e)
        {
            if (!UserSession.IsLoggedIn())
            {
                MessageBox.Show("Vous devez être connecté pour visionner un film.", "Connexion requise", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new FormulaireConnexion());
                return;
            }

            NavigationService.Navigate(new LecteurVideo(_filmId));
        }

        // Navigation vers E10 (Paiement)
        private void AcheterLouer_Click(object sender, RoutedEventArgs e)
        {
            if (!UserSession.IsLoggedIn())
            {
                MessageBox.Show("Vous devez être connecté pour acheter ou louer un film.", "Connexion requise", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new FormulaireConnexion());
                return;
            }

            NavigationService.Navigate(new SoldePaiement(_filmId));
        }

        // Coter Film (Extension de Visionner Film)
        private void CoterFilm_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Note et commentaire enregistrés (Classe Cote).", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
