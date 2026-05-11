using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using KasomaFlix.Application.DTOs;
using KasomaFlix.Application.UseCases.RechercheFilms;
using KasomaFlix.Presentation;
using KasomaFlix.Presentation.Services;

namespace KasomaFlix.Presentation.Views
{
    public partial class CatalogueMembre : Page
    {
        public CatalogueMembre()
        {
            InitializeComponent();
            Loaded += CatalogueMembre_Loaded;
        }

        private async void CatalogueMembre_Loaded(object sender, RoutedEventArgs e)
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

            if (!string.IsNullOrEmpty(film.CheminAffiche))
            {
                try
                {
                    image.Source = new BitmapImage(new Uri(film.CheminAffiche, UriKind.RelativeOrAbsolute));
                }
                catch
                {
                    // Image par défaut si le chemin est invalide
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
            NavigationService.Navigate(new MonCompte());
        }

        private void Connexion_Click(object sender, RoutedEventArgs e)
        {
            UserSession.Logout();
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
    }
}
