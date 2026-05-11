using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using KasomaFlix.Application.UseCases.ConsultationFilm;
using KasomaFlix.Presentation;
using KasomaFlix.Presentation.Services;
using System.Collections.Generic;
using System.Linq;

namespace KasomaFlix.Presentation.Views
{
    public partial class DetailsFilm : Page
    {
        private int _filmId;
        private decimal _prixAchat;
        private decimal _prixLocation;
        private string _titreFilm = string.Empty;

        public DetailsFilm(int filmId)
        {
            InitializeComponent();
            _filmId = filmId;
            Loaded += DetailsFilm_Loaded;
        }

        private async void DetailsFilm_Loaded(object sender, RoutedEventArgs e)
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

                    // Afficher les informations
                    TxtTitre.Text = film.Titre;
                    TxtInfos.Text = $"Catégorie: {film.Categorie} | Durée: {film.Duree} min | Année: {film.Annee}";
                    TxtNote.Text = $"Note: {film.NoteMoyenne:F1}/5";
                    TxtVotes.Text = $" ({film.NombreVotes} votes)";
                    TxtDescription.Text = film.Description;
                    TxtRealisateur.Text = $"Réalisateur: {film.Realisateur}";
                    TxtActeurs.Text = $"Acteurs: {film.Acteurs}";

                    // Stocker les prix et le titre pour la boîte de dialogue
                    _prixAchat = film.PrixAchat;
                    _prixLocation = film.PrixLocation;
                    _titreFilm = film.Titre;

                    // Charger l'affiche avec la même logique que le catalogue
                    ChargerImageAffiche(film.CheminAffiche);

                    // Stocker l'ID du film dans le Tag du bouton Visionner
                    BtnVisionner.Tag = film.Id;
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

        private void Visionner_Click(object sender, RoutedEventArgs e)
        {
            if (!UserSession.IsLoggedIn())
            {
                MessageBox.Show("Vous devez être connecté pour visionner un film.", "Connexion requise", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new FormulaireConnexion());
                return;
            }

            var button = sender as Button;
            if (button?.Tag is int filmId)
            {
                NavigationService.Navigate(new LecteurVideo(filmId));
            }
        }

        private void AcheterLouer_Click(object sender, RoutedEventArgs e)
        {
            if (!UserSession.IsLoggedIn())
            {
                MessageBox.Show("Vous devez être connecté pour acheter ou louer un film.", "Connexion requise", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new FormulaireConnexion());
                return;
            }

            // Debug pour vérifier les valeurs
            System.Diagnostics.Debug.WriteLine($"DetailsFilm - AcheterLouer_Click - FilmId: {_filmId}, Titre: {_titreFilm}, PrixAchat: {_prixAchat}, PrixLocation: {_prixLocation}");
            
            // Ouvrir la boîte de dialogue d'achat/location
            var dialog = new DialogAcheterLouer(_filmId, _titreFilm, _prixAchat, _prixLocation);
            dialog.Owner = System.Windows.Application.Current.MainWindow;
            dialog.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            
            var result = dialog.ShowDialog();
            
            if (result == true && dialog.TransactionReussie)
            {
                // Transaction réussie : recharger les détails ou afficher un message
                MessageBox.Show(
                    $"Transaction réussie !\nType : {dialog.TypeTransaction}\nMontant : {dialog.MontantTransaction:F2} $",
                    "Succès",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void CoterFilm_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Note et commentaire enregistrés (Classe Cote).", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Charge l'image de l'affiche du film avec la même logique que le catalogue
        /// </summary>
        private void ChargerImageAffiche(string? cheminAffiche)
        {
            if (string.IsNullOrWhiteSpace(cheminAffiche))
            {
                return;
            }

            try
            {
                BitmapImage? bitmapImage = null;
                
                // Normaliser le chemin : extraire le nom de fichier
                string nomFichier = cheminAffiche;
                
                // Enlever les préfixes /Images/ ou Images/
                if (nomFichier.StartsWith("/Images/", StringComparison.OrdinalIgnoreCase))
                {
                    nomFichier = nomFichier.Substring(8); 
                }
                else if (nomFichier.StartsWith("Images/", StringComparison.OrdinalIgnoreCase))
                {
                    nomFichier = nomFichier.Substring(7);
                }
                
                // Chercher le fichier dans le répertoire d'exécution
                var baseDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                
                // Si le dossier n'existe pas, essayer directement avec pack URI
                if (!System.IO.Directory.Exists(baseDir))
                {
                    // Essayer avec pack URI directement
                    var extensionsPack = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                    foreach (var ext in extensionsPack)
                    {
                        try
                        {
                            var packUri = new Uri($"pack://application:,,,/Images/{nomFichier.Replace(System.IO.Path.GetExtension(nomFichier), ext)}");
                            bitmapImage = new BitmapImage(packUri);
                            break;
                        }
                        catch { }
                    }
                    
                    if (bitmapImage != null)
                    {
                        ImgAffiche.Source = bitmapImage;
                    }
                    return;
                }
                
                var nomSansExt = System.IO.Path.GetFileNameWithoutExtension(nomFichier);
                var extensionOriginale = System.IO.Path.GetExtension(nomFichier);
                
                // Essayer différentes extensions (.webp -> .jpg, etc.)
                var extensions = new List<string> { extensionOriginale };
                if (extensionOriginale.Equals(".webp", StringComparison.OrdinalIgnoreCase))
                {
                    extensions.AddRange(new[] { ".jpg", ".jpeg", ".png" });
                }
                else if (extensionOriginale.Equals(".jpg", StringComparison.OrdinalIgnoreCase) || 
                         extensionOriginale.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    extensions.AddRange(new[] { ".webp", ".png" });
                }
                else if (string.IsNullOrEmpty(extensionOriginale))
                {
                    extensions.AddRange(new[] { ".jpg", ".jpeg", ".png", ".webp" });
                }
                
                string? fichierTrouve = null;
                
                // D'abord, essayer avec le nom exact (avec espaces, etc.)
                foreach (var ext in extensions.Distinct())
                {
                    var cheminTest = System.IO.Path.Combine(baseDir, nomSansExt + ext);
                    if (System.IO.File.Exists(cheminTest))
                    {
                        fichierTrouve = cheminTest;
                        break;
                    }
                }
                
                // Si pas trouvé, essayer avec le nom original complet (avec extension)
                if (fichierTrouve == null)
                {
                    var cheminOriginal = System.IO.Path.Combine(baseDir, nomFichier);
                    if (System.IO.File.Exists(cheminOriginal))
                    {
                        fichierTrouve = cheminOriginal;
                    }
                }
                
                // Si toujours pas trouvé, chercher dans tous les fichiers du dossier
                if (fichierTrouve == null)
                {
                    var fichiers = System.IO.Directory.GetFiles(baseDir, "*.*", System.IO.SearchOption.TopDirectoryOnly);
                    var nomNormalise = nomSansExt.Replace(" ", "").Replace("-", "").ToLower();
                    
                    foreach (var fichier in fichiers)
                    {
                        var nomFichierSansExt = System.IO.Path.GetFileNameWithoutExtension(fichier);
                        var nomFichierNormalise = nomFichierSansExt.Replace(" ", "").Replace("-", "").ToLower();
                        
                        if (nomFichierNormalise.Equals(nomNormalise, StringComparison.OrdinalIgnoreCase) ||
                            nomFichierSansExt.Equals(nomSansExt, StringComparison.OrdinalIgnoreCase))
                        {
                            fichierTrouve = fichier;
                            break;
                        }
                    }
                }
                
                // Si fichier trouvé, charger depuis le chemin absolu
                if (fichierTrouve != null)
                {
                    bitmapImage = new BitmapImage(new Uri(fichierTrouve));
                }
                // Sinon, essayer avec pack URI
                else
                {
                    foreach (var ext in extensions.Distinct())
                    {
                        try
                        {
                            var packUri = new Uri($"pack://application:,,,/Images/{nomSansExt}{ext}");
                            bitmapImage = new BitmapImage(packUri);
                            break;
                        }
                        catch { }
                    }
                    
                    // Si toujours pas trouvé, essayer avec le nom complet
                    if (bitmapImage == null)
                    {
                        try
                        {
                            var packUri = new Uri($"pack://application:,,,/Images/{nomFichier}");
                            bitmapImage = new BitmapImage(packUri);
                        }
                        catch { }
                    }
                }
                
                if (bitmapImage != null)
                {
                    ImgAffiche.Source = bitmapImage;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Image non trouvée pour: {cheminAffiche} (nomFichier: {nomFichier})");
                }
            }
            catch (Exception ex)
            {
                // Image par défaut si le chemin est invalide
                System.Diagnostics.Debug.WriteLine($"Erreur chargement image affiche ({cheminAffiche}): {ex.Message}");
            }
        }
    }
}
