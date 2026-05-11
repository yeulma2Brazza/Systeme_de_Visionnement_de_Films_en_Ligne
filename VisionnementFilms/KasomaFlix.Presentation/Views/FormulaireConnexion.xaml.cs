using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;
using KasomaFlix.Application.DTOs;
using KasomaFlix.Application.UseCases.Connexion;
using KasomaFlix.Presentation;
using KasomaFlix.Presentation.Services;

namespace KasomaFlix.Presentation.Views
{
    
    public partial class FormulaireConnexion : Page
    {
        public FormulaireConnexion()
        {
            InitializeComponent();
        }

        private void Accueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AccueilCatalogue());
        }

        private void Inscription_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FormulaireInscription());
        }

        private async void SeConnecter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Créer le DTO avec les données du formulaire
                var dto = new ConnexionDTO
                {
                    Identifiant = TxtIdentifiant.Text.Trim(),
                    MotDePasse = PwdMotDePasse.Password
                };

                // Créer un scope pour isoler cette opération
                using (var scope = ServiceLocator.CreateScope())
                {
                    var connexionUseCase = scope.ServiceProvider.GetRequiredService<ConnexionUseCase>();
                    var resultat = await connexionUseCase.ExecuteAsync(dto);

                    if (resultat.Succes)
                    {
                        // Enregistrer la session utilisateur
                        UserSession.SetCurrentUser(resultat);
                        
                        MessageBox.Show(resultat.Message, "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        // Redirection selon le type d'utilisateur
                        if (resultat.TypeUtilisateur == "Administrateur")
                        {
                            NavigationService.Navigate(new TableauBordAdmin());
                        }
                        else // Membre
                        {
                            NavigationService.Navigate(new CatalogueMembre());
                        }
                    }
                    else
                    {
                        MessageBox.Show(resultat.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur est survenue : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MotDePasseOublie_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fonctionnalité de récupération de mot de passe (TP3).", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
