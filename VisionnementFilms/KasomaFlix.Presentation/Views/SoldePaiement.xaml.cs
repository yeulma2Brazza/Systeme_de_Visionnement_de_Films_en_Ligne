using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;
using KasomaFlix.Application.DTOs;
using KasomaFlix.Application.UseCases.GestionTransactions;
using KasomaFlix.Application.UseCases.GestionProfil;
using KasomaFlix.Application.UseCases.GestionPaiements;
using KasomaFlix.Presentation;
using KasomaFlix.Presentation.Services;

namespace KasomaFlix.Presentation.Views
{
    public partial class SoldePaiement : Page
    {
        private int? _filmId;

        public SoldePaiement(int? filmId = null)
        {
            InitializeComponent();
            _filmId = filmId;
            Loaded += SoldePaiement_Loaded;
        }

        private async void SoldePaiement_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UserSession.IsLoggedIn() || !UserSession.GetUserId().HasValue)
            {
                NavigationService.Navigate(new FormulaireConnexion());
                return;
            }

            await ChargerSoldeAsync();
            await ChargerCartesCreditAsync();
        }

        private async Task ChargerSoldeAsync()
        {
            try
            {
                var userId = UserSession.GetUserId().Value;
                
                // Créer un scope pour isoler cette opération
                using (var scope = ServiceLocator.CreateScope())
                {
                    var obtenirProfilUseCase = scope.ServiceProvider.GetRequiredService<ObtenirProfilUseCase>();
                    var profil = await obtenirProfilUseCase.ExecuteAsync(userId);
                    if (profil != null)
                    {
                        TxtSolde.Text = $"{profil.Solde:F2} $";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du solde : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private async void AjouterSolde_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!UserSession.IsLoggedIn() || !UserSession.GetUserId().HasValue)
                {
                    MessageBox.Show("Vous devez être connecté.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!decimal.TryParse(TxtMontant.Text, out decimal montant) || montant <= 0)
                {
                    MessageBox.Show("Veuillez entrer un montant valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var dto = new CreerTransactionDTO
                {
                    MembreId = UserSession.GetUserId().Value,
                    FilmId = null,
                    TypeTransaction = "AjoutSolde",
                    Montant = montant
                };

                // Créer un scope pour isoler cette opération
                using (var scope = ServiceLocator.CreateScope())
                {
                    var creerTransactionUseCase = scope.ServiceProvider.GetRequiredService<CreerTransactionUseCase>();
                    var resultat = await creerTransactionUseCase.ExecuteAsync(dto);

                    if (resultat.Succes)
                    {
                        MessageBox.Show(resultat.Message, "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        await ChargerSoldeAsync();
                    }
                    else
                    {
                        MessageBox.Show(resultat.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ChargerCartesCreditAsync()
        {
            try
            {
                if (!UserSession.IsLoggedIn() || !UserSession.GetUserId().HasValue)
                {
                    return;
                }

                var userId = UserSession.GetUserId().Value;
                
                using (var scope = ServiceLocator.CreateScope())
                {
                    var obtenirCartesCreditUseCase = scope.ServiceProvider.GetRequiredService<ObtenirCartesCreditUseCase>();
                    var cartes = await obtenirCartesCreditUseCase.ExecuteAsync(userId);

                    // Vider le panel
                    PanelCartesCredit.Children.Clear();

                    if (cartes.Any())
                    {
                        TxtAucuneCarte.Visibility = Visibility.Collapsed;
                        
                        foreach (var carte in cartes)
                        {
                            string texteCarte = string.Empty;
                            
                            if (carte.TypeCarte == "PayPal")
                            {
                                texteCarte = $"PayPal ({carte.EmailPayPal})";
                            }
                            else
                            {
                                string moisExpiration = carte.DateExpiration.ToString("MM");
                                string anneeExpiration = carte.DateExpiration.ToString("yy");
                                texteCarte = $"Carte {carte.TypeCarte} se terminant par {carte.NumeroCarteMasque} (Expire {moisExpiration}/{anneeExpiration})";
                                
                                if (carte.EstParDefaut)
                                {
                                    texteCarte += " [Par défaut]";
                                }
                            }

                            var textBlock = new TextBlock
                            {
                                Text = texteCarte,
                                Foreground = System.Windows.Media.Brushes.White,
                                Margin = new Thickness(0, 5, 0, 5)
                            };
                            
                            PanelCartesCredit.Children.Add(textBlock);
                        }
                    }
                    else
                    {
                        TxtAucuneCarte.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du chargement des cartes : {ex.Message}");
            }
        }

        private void AjouterPaiement_Click(object sender, RoutedEventArgs e)
        {
            if (!UserSession.IsLoggedIn())
            {
                MessageBox.Show("Vous devez être connecté pour ajouter un mode de paiement.", "Connexion requise", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new FormulaireConnexion());
                return;
            }

            // Ouvrir la boîte de dialogue d'ajout de carte
            var dialog = new DialogAjouterCarteCredit();
            dialog.Owner = System.Windows.Application.Current.MainWindow;
            dialog.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            
            var result = dialog.ShowDialog();
            
            if (result == true && dialog.CarteAjoutee)
            {
                // Recharger la liste des cartes
                _ = ChargerCartesCreditAsync();
            }
        }
    }
}
