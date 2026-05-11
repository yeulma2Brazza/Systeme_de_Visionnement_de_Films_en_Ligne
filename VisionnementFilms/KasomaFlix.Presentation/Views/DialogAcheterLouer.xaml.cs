using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using KasomaFlix.Application.DTOs;
using KasomaFlix.Application.UseCases.GestionTransactions;
using KasomaFlix.Application.UseCases.GestionProfil;
using KasomaFlix.Presentation;
using KasomaFlix.Presentation.Services;

namespace KasomaFlix.Presentation.Views
{
    public partial class DialogAcheterLouer : Window
    {
        private int _filmId;
        private decimal _prixAchat;
        private decimal _prixLocation;
        private string _titreFilm;
        private bool _estAchat = true; // Par défaut, achat sélectionné

        public bool TransactionReussie { get; private set; } = false;
        public string TypeTransaction { get; private set; } = string.Empty;
        public decimal MontantTransaction { get; private set; } = 0;

        public DialogAcheterLouer(int filmId, string titreFilm, decimal prixAchat, decimal prixLocation)
        {
            InitializeComponent();
            _filmId = filmId;
            _titreFilm = titreFilm;
            _prixAchat = prixAchat;
            _prixLocation = prixLocation;

            TxtTitreFilm.Text = titreFilm;
            TxtPrixAchat.Text = $"Prix: {prixAchat:F2} $";
            TxtPrixLocation.Text = $"Prix: {prixLocation:F2} $";
            
            // Debug pour vérifier les valeurs
            System.Diagnostics.Debug.WriteLine($"DialogAcheterLouer - FilmId: {filmId}, PrixAchat: {prixAchat}, PrixLocation: {prixLocation}");

            Loaded += DialogAcheterLouer_Loaded;
        }

        private async void DialogAcheterLouer_Loaded(object sender, RoutedEventArgs e)
        {
            // Attendre que tous les contrôles soient initialisés
            // Les contrôles sont initialisés après InitializeComponent(), donc on peut appeler MettreAJourInterface
            // Mais on doit s'assurer que le solde est chargé avant de vérifier
            await ChargerSoldeAsync();
            // Mettre à jour l'interface après le chargement du solde
            MettreAJourInterface();
        }

        private async Task ChargerSoldeAsync()
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
                    var obtenirProfilUseCase = scope.ServiceProvider.GetRequiredService<ObtenirProfilUseCase>();
                    var profil = await obtenirProfilUseCase.ExecuteAsync(userId);
                    if (profil != null)
                    {
                        TxtSoldeActuel.Text = $"{profil.Solde:F2} $";
                        MettreAJourInterface();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du chargement du solde : {ex.Message}");
            }
        }

        private void RadioAchat_Checked(object sender, RoutedEventArgs e)
        {
            _estAchat = true;
            MettreAJourInterface();
        }

        private void RadioLocation_Checked(object sender, RoutedEventArgs e)
        {
            _estAchat = false;
            MettreAJourInterface();
        }

        private void BorderAchat_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RadioAchat.IsChecked = true;
        }

        private void BorderLocation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RadioLocation.IsChecked = true;
        }

        private void MettreAJourInterface()
        {
            // Vérifier que tous les contrôles sont initialisés avant de les utiliser
            if (BorderAchat == null || BorderLocation == null || 
                CheckAchat == null || CheckLocation == null || 
                TxtSoldeActuel == null || TxtMessageErreur == null || 
                BtnConfirmer == null)
            {
                // Les contrôles ne sont pas encore initialisés, on sort de la méthode
                return;
            }

            // Mettre à jour les bordures et les checkmarks
            if (_estAchat)
            {
                BorderAchat.BorderBrush = new SolidColorBrush(Color.FromRgb(229, 9, 20)); // #E50914
                BorderAchat.BorderThickness = new Thickness(2);
                CheckAchat.Visibility = Visibility.Visible;
                
                BorderLocation.BorderBrush = new SolidColorBrush(Color.FromRgb(68, 68, 68)); // #444444
                BorderLocation.BorderThickness = new Thickness(1);
                CheckLocation.Visibility = Visibility.Collapsed;
            }
            else
            {
                BorderLocation.BorderBrush = new SolidColorBrush(Color.FromRgb(229, 9, 20)); // #E50914
                BorderLocation.BorderThickness = new Thickness(2);
                CheckLocation.Visibility = Visibility.Visible;
                
                BorderAchat.BorderBrush = new SolidColorBrush(Color.FromRgb(68, 68, 68)); // #444444
                BorderAchat.BorderThickness = new Thickness(1);
                CheckAchat.Visibility = Visibility.Collapsed;
            }

            // Vérifier le solde et afficher un message si insuffisant
            decimal montantRequis = _estAchat ? _prixAchat : _prixLocation;
            string typeOperation = _estAchat ? "achat" : "location";
            
            // Récupérer le solde actuel depuis le TextBlock (avec vérification null)
            if (TxtSoldeActuel != null && !string.IsNullOrEmpty(TxtSoldeActuel.Text))
            {
                string soldeText = TxtSoldeActuel.Text.Replace(" $", "").Trim();
                if (decimal.TryParse(soldeText, out decimal soldeActuel))
                {
                    if (soldeActuel < montantRequis)
                    {
                        if (TxtMessageErreur != null)
                        {
                            TxtMessageErreur.Text = $"Solde insuffisant pour effectuer cette {typeOperation}. Solde actuel : {soldeActuel:F2} $. Montant requis : {montantRequis:F2} $.";
                            TxtMessageErreur.Visibility = Visibility.Visible;
                        }
                        if (BtnConfirmer != null)
                        {
                            BtnConfirmer.IsEnabled = false;
                            BtnConfirmer.Background = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                        }
                    }
                    else
                    {
                        if (TxtMessageErreur != null)
                        {
                            TxtMessageErreur.Visibility = Visibility.Collapsed;
                        }
                        if (BtnConfirmer != null)
                        {
                            BtnConfirmer.IsEnabled = true;
                            BtnConfirmer.Background = new SolidColorBrush(Color.FromRgb(229, 9, 20)); // #E50914
                        }
                    }
                }
                else
                {
                    // Si le solde n'est pas encore chargé, désactiver temporairement le bouton
                    if (BtnConfirmer != null)
                    {
                        BtnConfirmer.IsEnabled = false;
                        BtnConfirmer.Background = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                    }
                }
            }
            else
            {
                // Si le solde n'est pas encore chargé, désactiver temporairement le bouton
                if (BtnConfirmer != null)
                {
                    BtnConfirmer.IsEnabled = false;
                    BtnConfirmer.Background = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                }
            }
        }

        private async void Confirmer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!UserSession.IsLoggedIn() || !UserSession.GetUserId().HasValue)
                {
                    MessageBox.Show("Vous devez être connecté pour effectuer cette opération.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    DialogResult = false;
                    return;
                }

                var userId = UserSession.GetUserId().Value;
                decimal montant = _estAchat ? _prixAchat : _prixLocation;
                string typeTransaction = _estAchat ? "Achat" : "Location";

                // Confirmation
                var confirmation = MessageBox.Show(
                    $"Confirmez-vous le {typeTransaction.ToLower()} de \"{_titreFilm}\" pour {montant:F2} $ ?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirmation != MessageBoxResult.Yes)
                {
                    return;
                }

                // Créer la transaction
                using (var scope = ServiceLocator.CreateScope())
                {
                    var creerTransactionUseCase = scope.ServiceProvider.GetRequiredService<CreerTransactionUseCase>();
                    
                    var dto = new CreerTransactionDTO
                    {
                        MembreId = userId,
                        FilmId = _filmId,
                        TypeTransaction = typeTransaction,
                        Montant = montant
                    };

                    var resultat = await creerTransactionUseCase.ExecuteAsync(dto);

                    if (resultat.Succes)
                    {
                        TransactionReussie = true;
                        TypeTransaction = typeTransaction;
                        MontantTransaction = montant;
                        
                        // Recharger le solde pour afficher la mise à jour
                        await ChargerSoldeAsync();
                        
                        MessageBox.Show(
                            $"{typeTransaction} effectué avec succès !\n\nMontant débité : {montant:F2} $\nNouveau solde : {TxtSoldeActuel.Text}",
                            "Succès",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show(resultat.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la transaction : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
