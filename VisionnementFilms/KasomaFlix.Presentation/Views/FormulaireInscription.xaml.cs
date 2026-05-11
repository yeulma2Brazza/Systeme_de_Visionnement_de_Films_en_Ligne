using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;
using KasomaFlix.Application.DTOs;
using KasomaFlix.Application.UseCases.Inscription;
using KasomaFlix.Presentation;

namespace KasomaFlix.Presentation.Views
{
    
    public partial class FormulaireInscription : Page
    {
        public FormulaireInscription()
        {
            InitializeComponent();
        }

        private void Accueil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AccueilCatalogue());
        }

        private void Connexion_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FormulaireConnexion());
        }

        private async void ConfirmerInscription_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Créer le DTO avec les données du formulaire
                var dto = new InscriptionDTO
                {
                    Prenom = TxtPrenom.Text.Trim(),
                    Nom = TxtNom.Text.Trim(),
                    Courriel = TxtCourriel.Text.Trim(),
                    MotDePasse = PwdMotDePasse.Password,
                    Adresse = TxtAdresse.Text.Trim(),
                    Telephone = TxtTelephone.Text.Trim()
                };

                // Créer un scope pour isoler cette opération
                using (var scope = ServiceLocator.CreateScope())
                {
                    var inscriptionUseCase = scope.ServiceProvider.GetRequiredService<InscriptionMembreUseCase>();
                    var resultat = await inscriptionUseCase.ExecuteAsync(dto);

                    if (resultat.Succes)
                    {
                        MessageBox.Show(resultat.Message, "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Redirection vers la page de connexion
                        NavigationService.Navigate(new FormulaireConnexion());
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

        private void SeConnecter_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FormulaireConnexion());
        }
    }
}
