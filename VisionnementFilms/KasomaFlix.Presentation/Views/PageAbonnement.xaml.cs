using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;
using KasomaFlix.Application.DTOs;
using KasomaFlix.Application.UseCases.GestionAbonnements;
using KasomaFlix.Application.UseCases.GestionProfil;
using KasomaFlix.Presentation;
using KasomaFlix.Presentation.Services;

namespace KasomaFlix.Presentation.Views
{
    public partial class PageAbonnement : Page
    {
        private int? _abonnementActifId = null;

        public PageAbonnement()
        {
            InitializeComponent();
            Loaded += PageAbonnement_Loaded;
        }

        private async void PageAbonnement_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UserSession.IsLoggedIn() || !UserSession.GetUserId().HasValue)
            {
                NavigationService.Navigate(new FormulaireConnexion());
                return;
            }

            await ChargerAbonnementsAsync();
        }

        private async Task ChargerAbonnementsAsync()
        {
            try
            {
                var userId = UserSession.GetUserId().Value;
                
                // Créer un scope pour isoler cette opération
                using (var scope = ServiceLocator.CreateScope())
                {
                    var obtenirAbonnementsUseCase = scope.ServiceProvider.GetRequiredService<ObtenirAbonnementsUseCase>();
                    var obtenirProfilUseCase = scope.ServiceProvider.GetRequiredService<ObtenirProfilUseCase>();
                    
                    var abonnements = await obtenirAbonnementsUseCase.ExecuteAsync(userId);
                    var abonnementActif = abonnements.FirstOrDefault(a => a.EstActif);
                    var profil = await obtenirProfilUseCase.ExecuteAsync(userId);

                    if (abonnementActif != null)
                    {
                        _abonnementActifId = abonnementActif.Id;
                        TxtPlanActuel.Text = $"Plan : {abonnementActif.TypeAbonnement}";
                        TxtDateRenouvellement.Text = $"Prochain renouvellement : {abonnementActif.DateFin:dd MMMM yyyy}";
                        TxtDateRenouvellement.Visibility = Visibility.Visible;
                        BtnAnnulerRenouvellement.Visibility = Visibility.Visible;

                        // Mettre en évidence le plan actif
                        if (abonnementActif.TypeAbonnement.Contains("Premium", StringComparison.OrdinalIgnoreCase))
                        {
                            BorderPremium.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(229, 9, 20));
                            BtnPremium.Content = "Plan Actuel";
                            BtnPremium.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0));
                            BtnPremium.Foreground = System.Windows.Media.Brushes.Black;
                            BtnPremium.IsEnabled = false;
                        }
                        else if (abonnementActif.TypeAbonnement.Contains("Standard", StringComparison.OrdinalIgnoreCase))
                        {
                            BtnStandard.Content = "Plan Actuel";
                            BtnStandard.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0));
                            BtnStandard.Foreground = System.Windows.Media.Brushes.Black;
                            BtnStandard.IsEnabled = false;
                        }
                    }
                    else
                    {
                        TxtPlanActuel.Text = "Aucun abonnement actif";
                        TxtPlanActuel.Foreground = System.Windows.Media.Brushes.Orange;
                        TxtDateRenouvellement.Visibility = Visibility.Collapsed;
                        BtnAnnulerRenouvellement.Visibility = Visibility.Collapsed;
                    }

                    // Vérifier le solde pour afficher un avertissement si insuffisant
                    if (profil != null)
                    {
                        if (profil.Solde < 9.99m)
                        {
                            MessageBox.Show(
                                $"Votre solde est insuffisant pour activer un abonnement.\nSolde actuel : {profil.Solde:F2} $\n\nVeuillez ajouter des fonds dans la section 'Solde et Paiement'.",
                                "Solde Insuffisant",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des abonnements : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Accueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AccueilCatalogue());
        }

        private void MonCompte_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MonCompte());
        }

        private async void AnnulerRenouvellement_Click(object sender, RoutedEventArgs e)
        {
            if (!_abonnementActifId.HasValue)
            {
                MessageBox.Show("Aucun abonnement actif à modifier.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                "Êtes-vous sûr de vouloir annuler le renouvellement automatique ?\n\nVotre abonnement restera actif jusqu'à la date d'expiration, mais ne sera pas renouvelé automatiquement.",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var scope = ServiceLocator.CreateScope())
                    {
                        var annulerRenouvellementUseCase = scope.ServiceProvider.GetRequiredService<AnnulerRenouvellementUseCase>();
                        var success = await annulerRenouvellementUseCase.ExecuteAsync(_abonnementActifId.Value);
                        
                        if (success)
                        {
                            MessageBox.Show("Renouvellement automatique annulé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                            await ChargerAbonnementsAsync();
                        }
                        else
                        {
                            MessageBox.Show("Erreur lors de l'annulation du renouvellement.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void ChoisirPlanStandard_Click(object sender, RoutedEventArgs e)
        {
            await ActiverAbonnementAsync("Standard Mensuel", 9.99m);
        }

        private async void ChoisirPlanPremium_Click(object sender, RoutedEventArgs e)
        {
            await ActiverAbonnementAsync("Premium Mensuel", 14.99m);
        }

        private async Task ActiverAbonnementAsync(string typeAbonnement, decimal prix)
        {
            try
            {
                var userId = UserSession.GetUserId();
                if (!userId.HasValue)
                {
                    MessageBox.Show("Vous devez être connecté.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Vérifier le solde d'abord
                using (var scope = ServiceLocator.CreateScope())
                {
                    var obtenirProfilUseCase = scope.ServiceProvider.GetRequiredService<ObtenirProfilUseCase>();
                    var profil = await obtenirProfilUseCase.ExecuteAsync(userId.Value);

                    if (profil == null)
                    {
                        MessageBox.Show("Impossible de récupérer votre profil.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (profil.Solde < prix)
                    {
                        var result = MessageBox.Show(
                            $"Solde insuffisant pour activer l'abonnement {typeAbonnement}.\n\nSolde actuel : {profil.Solde:F2} $\nMontant requis : {prix:F2} $\n\nSouhaitez-vous ajouter des fonds maintenant ?",
                            "Solde Insuffisant",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Yes)
                        {
                            NavigationService.Navigate(new SoldePaiement());
                            return;
                        }
                        return;
                    }

                    // Confirmer l'activation
                    var confirmation = MessageBox.Show(
                        $"Voulez-vous activer l'abonnement {typeAbonnement} pour {prix:F2} $ ?\n\nVotre solde actuel sera débité de {prix:F2} $.",
                        "Confirmation d'Abonnement",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (confirmation == MessageBoxResult.Yes)
                    {
                        var creerAbonnementUseCase = scope.ServiceProvider.GetRequiredService<CreerAbonnementUseCase>();
                        
                        var dto = new CreerAbonnementDTO
                        {
                            MembreId = userId.Value,
                            TypeAbonnement = typeAbonnement,
                            Prix = prix,
                            RenouvellementAutomatique = true
                        };

                        var resultat = await creerAbonnementUseCase.ExecuteAsync(dto);

                        if (resultat.Succes)
                        {
                            MessageBox.Show(resultat.Message, "Abonnement Activé", MessageBoxButton.OK, MessageBoxImage.Information);
                            await ChargerAbonnementsAsync();
                        }
                        else
                        {
                            MessageBox.Show(resultat.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'activation de l'abonnement : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
