using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using KasomaFlix.Application.DTOs;
using KasomaFlix.Application.UseCases.RechercheFilms;
using KasomaFlix.Presentation;
using System.Collections.Generic;
using System.Linq;

namespace KasomaFlix.Presentation.Views
{
    public partial class AccueilCatalogue : Page
    {
        public AccueilCatalogue()
        {
            InitializeComponent();
            Loaded += AccueilCatalogue_Loaded;
        }

        private async void AccueilCatalogue_Loaded(object sender, RoutedEventArgs e)
        {
            await ChargerFilmsAsync();
        }

        private async Task ChargerFilmsAsync(string? recherche = null)
        {
            try
            {
                MovieGrid.Children.Clear();

                RechercheFilmsDTO? criteres = null;
                if (!string.IsNullOrWhiteSpace(recherche) && recherche != "Rechercher films, séries...")
                {
                    criteres = new RechercheFilmsDTO { Titre = recherche };
                }

                // Créer un scope pour isoler cette opération
                using (var scope = ServiceLocator.CreateScope())
                {
                    var rechercherFilmsUseCase = scope.ServiceProvider.GetRequiredService<RechercherFilmsUseCase>();
                    var films = await rechercherFilmsUseCase.ExecuteAsync(criteres);

                    foreach (var film in films)
                    {
                        var filmCard = CreerCarteFilm(film);
                        MovieGrid.Children.Add(filmCard);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des films : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Border CreerCarteFilm(FilmDTO film)
        {
            var border = new Border
            {
                Width = 180,
                Height = 300,
                Margin = new Thickness(10),
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#222")),
                CornerRadius = new CornerRadius(5),
                Cursor = System.Windows.Input.Cursors.Hand,
                Tag = film.Id
            };

            var stackPanel = new StackPanel();

            // Image
            var image = new Image
            {
                Height = 200,
                Stretch = System.Windows.Media.Stretch.UniformToFill
            };

            // Construire le chemin de l'image
            string imagePath = ObtenirCheminImage(film);
            
            if (!string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    BitmapImage? bitmapImage = null;
                    
                    // Normaliser le chemin : extraire le nom de fichier
                    string nomFichier = imagePath;
                    
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
                    var nomSansExt = System.IO.Path.GetFileNameWithoutExtension(nomFichier);
                    var extensionOriginale = System.IO.Path.GetExtension(nomFichier);
                    
                    // Essayer différentes extensions (.webp -> .jpg, etc.)
                    var extensions = new List<string> { extensionOriginale };
                    if (extensionOriginale.Equals(".webp", StringComparison.OrdinalIgnoreCase))
                    {
                        extensions.AddRange(new[] { ".jpg", ".jpeg" });
                    }
                    else if (extensionOriginale.Equals(".jpg", StringComparison.OrdinalIgnoreCase) || 
                             extensionOriginale.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
                    {
                        extensions.AddRange(new[] { ".webp", ".png" });
                    }
                    
                    string? fichierTrouve = null;
                    foreach (var ext in extensions.Distinct())
                    {
                        var cheminTest = System.IO.Path.Combine(baseDir, nomSansExt + ext);
                        if (System.IO.File.Exists(cheminTest))
                        {
                            fichierTrouve = cheminTest;
                            break;
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
                    }
                    
                    if (bitmapImage != null)
                    {
                        image.Source = bitmapImage;
                    }
                }
                catch (Exception ex)
                {
                    // Image par défaut si le chemin est invalide
                    System.Diagnostics.Debug.WriteLine($"Erreur chargement image pour {film.Titre} ({imagePath}): {ex.Message}");
                }
            }

            stackPanel.Children.Add(image);

            // Informations
            var infoPanel = new StackPanel { Margin = new Thickness(10) };
            
            var titre = new TextBlock
            {
                Text = film.Titre,
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap
            };
            infoPanel.Children.Add(titre);

            var note = new TextBlock
            {
                Text = $"Note: {film.NoteMoyenne:F1}/5",
                Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E50914")),
                FontSize = 12
            };
            infoPanel.Children.Add(note);

            var categorie = new TextBlock
            {
                Text = film.Categorie,
                Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#AAAAAA")),
                FontSize = 12
            };
            infoPanel.Children.Add(categorie);

            stackPanel.Children.Add(infoPanel);
            border.Child = stackPanel;
            border.MouseLeftButtonDown += Film_Click;

            return border;
        }

        private void Film_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border?.Tag is int filmId)
            {
                NavigationService?.Navigate(new DetailsFilm(filmId));
            }
        }

        private void Inscription_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FormulaireInscription());
        }

        private void Connexion_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FormulaireConnexion());
        }

        private void TxtRecherche_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtRecherche.Text == "Rechercher films, séries...")
            {
                TxtRecherche.Text = "";
                TxtRecherche.Foreground = System.Windows.Media.Brushes.White;
            }
        }

        private void TxtRecherche_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtRecherche.Text))
            {
                TxtRecherche.Text = "Rechercher films, séries...";
                TxtRecherche.Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#AAAAAA"));
            }
        }

        private async void BtnRechercher_Click(object sender, RoutedEventArgs e)
        {
            string? recherche = TxtRecherche.Text;
            if (recherche == "Rechercher films, séries...")
            {
                recherche = null;
            }
            await ChargerFilmsAsync(recherche);
        }

        /// <summary>
        /// Obtient le chemin de l'image pour un film donné
        /// </summary>
        private string ObtenirCheminImage(FilmDTO film)
        {
            // Si un chemin est déjà fourni dans la base de données, l'utiliser tel quel
            if (!string.IsNullOrWhiteSpace(film.CheminAffiche))
            {
                return film.CheminAffiche;
            }

            // Sinon, essayer de mapper le titre du film au nom de fichier
            var titreNormalise = film.Titre
                .Replace(" ", "")
                .Replace("'", "")
                .Replace("-", "")
                .Replace(":", "")
                .Replace(",", "")
                .ToLower();

            // Mapping des titres aux noms de fichiers (basé sur les fichiers réels dans Images/)
            var mapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "lespetitesvictoires", "les petites victoires.jpg" },
                { "mayday", "Mayday.jpg" },
                { "rémisansfamille", "Remi.jpg" },
                { "remisansfamille", "Remi.jpg" },
                { "smile", "smile.jpg" },
                { "blockbusters2025", "autres.jpg" },
                { "balleperdue", "Balle perdue.jpeg" },
                { "frères", "Freres.jpg" },
                { "freres", "Freres.jpg" },
                { "jokerfolieàdeux", "Joker.jpg" },
                { "jokerfolieadeux", "Joker.jpg" },
                { "oppenheimer", "Oppenheimer.jpg" }
            };

            if (mapping.TryGetValue(titreNormalise, out var nomFichier))
            {
                return nomFichier;
            }

            // Essayer de trouver un fichier qui correspond au titre
            var baseDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            if (System.IO.Directory.Exists(baseDir))
            {
                var fichiers = System.IO.Directory.GetFiles(baseDir, "*.*", System.IO.SearchOption.TopDirectoryOnly);
                var fichierCorrespondant = fichiers.FirstOrDefault(f => 
                    System.IO.Path.GetFileNameWithoutExtension(f).Equals(film.Titre, StringComparison.OrdinalIgnoreCase) ||
                    System.IO.Path.GetFileNameWithoutExtension(f).Replace(" ", "").Equals(titreNormalise, StringComparison.OrdinalIgnoreCase));

                if (fichierCorrespondant != null)
                {
                    return System.IO.Path.GetFileName(fichierCorrespondant);
                }
            }

            return string.Empty;
        }
    }
}
