using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using KasomaFlix.Application.DTOs;
using KasomaFlix.Application.UseCases.GestionPaiements;
using KasomaFlix.Presentation;
using KasomaFlix.Presentation.Services;

namespace KasomaFlix.Presentation.Views
{
    public partial class DialogAjouterCarteCredit : Window
    {
        public bool CarteAjoutee { get; private set; } = false;

        public DialogAjouterCarteCredit()
        {
            InitializeComponent();
            Loaded += DialogAjouterCarteCredit_Loaded;
        }

        private void DialogAjouterCarteCredit_Loaded(object sender, RoutedEventArgs e)
        {
            // Remplir les années (année actuelle + 10 ans)
            var anneeActuelle = DateTime.Now.Year;
            for (int i = 0; i <= 10; i++)
            {
                var annee = anneeActuelle + i;
                var item = new ComboBoxItem { Content = annee.ToString(), Tag = annee };
                CmbAnnee.Items.Add(item);
            }
            CmbAnnee.SelectedIndex = 0;

            // Sélectionner le premier type de carte
            CmbTypeCarte.SelectedIndex = 0;
        }

        private void CmbTypeCarte_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbTypeCarte.SelectedItem is ComboBoxItem selectedItem)
            {
                string typeCarte = selectedItem.Tag?.ToString() ?? string.Empty;
                
                // Afficher/masquer les champs selon le type
                if (typeCarte == "PayPal")
                {
                    PanelNumeroCarte.Visibility = Visibility.Collapsed;
                    PanelDateExpiration.Visibility = Visibility.Collapsed;
                    PanelEmailPayPal.Visibility = Visibility.Visible;
                }
                else
                {
                    PanelNumeroCarte.Visibility = Visibility.Visible;
                    PanelDateExpiration.Visibility = Visibility.Visible;
                    PanelEmailPayPal.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void TxtNumeroCarte_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Permettre seulement les chiffres et les espaces
            Regex regex = new Regex("[^0-9 ]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!UserSession.IsLoggedIn() || !UserSession.GetUserId().HasValue)
                {
                    MessageBox.Show("Vous devez être connecté pour ajouter un mode de paiement.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    DialogResult = false;
                    return;
                }

                var userId = UserSession.GetUserId().Value;

                // Récupérer le type de carte
                if (CmbTypeCarte.SelectedItem is not ComboBoxItem selectedType)
                {
                    AfficherErreur("Veuillez sélectionner un type de mode de paiement.");
                    return;
                }

                string typeCarte = selectedType.Tag?.ToString() ?? string.Empty;

                // Validation selon le type
                if (typeCarte == "PayPal")
                {
                    if (string.IsNullOrWhiteSpace(TxtEmailPayPal.Text))
                    {
                        AfficherErreur("Veuillez entrer votre adresse email PayPal.");
                        return;
                    }

                    if (!TxtEmailPayPal.Text.Contains("@"))
                    {
                        AfficherErreur("Veuillez entrer une adresse email valide.");
                        return;
                    }
                }
                else
                {
                    // Validation pour les cartes de crédit
                    if (string.IsNullOrWhiteSpace(TxtNumeroCarte.Text) || TxtNumeroCarte.Text.Replace(" ", "").Length < 13)
                    {
                        AfficherErreur("Veuillez entrer un numéro de carte valide (minimum 13 chiffres).");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(TxtNomTitulaire.Text))
                    {
                        AfficherErreur("Veuillez entrer le nom du titulaire.");
                        return;
                    }

                    if (CmbMois.SelectedItem == null || CmbAnnee.SelectedItem == null)
                    {
                        AfficherErreur("Veuillez sélectionner la date d'expiration.");
                        return;
                    }
                }

                // Construire la date d'expiration
                DateTime dateExpiration = DateTime.Now;
                if (typeCarte != "PayPal")
                {
                    int mois = int.Parse((CmbMois.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "1");
                    int annee = int.Parse((CmbAnnee.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? DateTime.Now.Year.ToString());
                    dateExpiration = new DateTime(annee, mois, DateTime.DaysInMonth(annee, mois));
                }

                // Créer le DTO
                var dto = new CreerCarteCreditDTO
                {
                    MembreId = userId,
                    TypeCarte = typeCarte,
                    NumeroCarte = TxtNumeroCarte.Text.Replace(" ", ""), // Enlever les espaces
                    NomTitulaire = TxtNomTitulaire.Text,
                    DateExpiration = dateExpiration,
                    EmailPayPal = typeCarte == "PayPal" ? TxtEmailPayPal.Text : null,
                    EstParDefaut = ChkEstParDefaut.IsChecked ?? false
                };

                // Appeler le UseCase
                using (var scope = ServiceLocator.CreateScope())
                {
                    var ajouterCarteCreditUseCase = scope.ServiceProvider.GetRequiredService<AjouterCarteCreditUseCase>();
                    var resultat = await ajouterCarteCreditUseCase.ExecuteAsync(dto);

                    if (resultat.Succes)
                    {
                        CarteAjoutee = true;
                        MessageBox.Show(resultat.Message, "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        AfficherErreur(resultat.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                AfficherErreur($"Erreur lors de l'ajout du mode de paiement : {ex.Message}");
            }
        }

        private void AfficherErreur(string message)
        {
            TxtMessageErreur.Text = message;
            TxtMessageErreur.Visibility = Visibility.Visible;
        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
