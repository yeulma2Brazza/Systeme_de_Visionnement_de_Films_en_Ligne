using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;
using KasomaFlix.Application.DTOs;
using KasomaFlix.Application.UseCases.GestionProfil;
using KasomaFlix.Presentation;
using KasomaFlix.Presentation.Services;

namespace KasomaFlix.Presentation.Views
{
    public partial class ModificationProfil : Page
    {
        public ModificationProfil()
        {
            InitializeComponent();
            Loaded += ModificationProfil_Loaded;
        }

        private async void ModificationProfil_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UserSession.IsLoggedIn() || !UserSession.GetUserId().HasValue)
            {
                NavigationService.Navigate(new FormulaireConnexion());
                return;
            }

            await ChargerProfilAsync();
        }

        private async Task ChargerProfilAsync()
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
                        TxtPrenom.Text = profil.Prenom;
                        TxtNom.Text = profil.Nom;
                        TxtCourriel.Text = profil.Courriel;
                        TxtAdresse.Text = profil.Adresse;
                        TxtTelephone.Text = profil.Telephone;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du profil : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private async void Sauvegarder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!UserSession.IsLoggedIn() || !UserSession.GetUserId().HasValue)
                {
                    MessageBox.Show("Vous devez être connecté pour modifier votre profil.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Vérifier que les mots de passe correspondent si un nouveau mot de passe est fourni
                if (!string.IsNullOrWhiteSpace(PwdNouveauMotDePasse.Password))
                {
                    if (PwdNouveauMotDePasse.Password != PwdConfirmerMotDePasse.Password)
                    {
                        MessageBox.Show("Les mots de passe ne correspondent pas.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                var dto = new ModifierProfilDTO
                {
                    Prenom = TxtPrenom.Text.Trim(),
                    Nom = TxtNom.Text.Trim(),
                    Adresse = TxtAdresse.Text.Trim(),
                    Telephone = TxtTelephone.Text.Trim(),
                    NouveauMotDePasse = string.IsNullOrWhiteSpace(PwdNouveauMotDePasse.Password) ? null : PwdNouveauMotDePasse.Password
                };

                // Créer un scope pour isoler cette opération
                using (var scope = ServiceLocator.CreateScope())
                {
                    var modifierProfilUseCase = scope.ServiceProvider.GetRequiredService<ModifierProfilUseCase>();
                    var resultat = await modifierProfilUseCase.ExecuteAsync(UserSession.GetUserId().Value, dto);

                    if (resultat.Succes)
                    {
                        MessageBox.Show(resultat.Message, "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        NavigationService.Navigate(new MonCompte());
                    }
                    else
                    {
                        MessageBox.Show(resultat.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sauvegarde : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
