using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;
using KasomaFlix.Application.DTOs;
using KasomaFlix.Application.UseCases.GestionAdmin;
using KasomaFlix.Application.UseCases.RechercheFilms;
using KasomaFlix.Application.UseCases.ConsultationFilm;
using KasomaFlix.Presentation;
using KasomaFlix.Presentation.Services;

namespace KasomaFlix.Presentation.Views
{
    public partial class GestionFilm : Page
    {
        private int? _filmIdEnEdition = null;

        public GestionFilm(int? filmId = null)
        {
            InitializeComponent();
            _filmIdEnEdition = filmId;
            
            // Vérifier l'authentification admin
            if (!UserSession.IsLoggedIn() || !UserSession.IsAdmin())
            {
                NavigationService.Navigate(new FormulaireConnexion());
                return;
            }

            Loaded += GestionFilm_Loaded;
        }

        private async void GestionFilm_Loaded(object sender, RoutedEventArgs e)
        {
            await ChargerListeFilmsAsync();
            
            // Si un filmId a été passé en paramètre, charger ce film
            if (_filmIdEnEdition.HasValue)
            {
                await ChargerFilmAsync(_filmIdEnEdition.Value);
            }
            else
            {
                // Mode "Nouveau film" par défaut
                CmbFilmsExistants.SelectedIndex = 0;
                MettreAJourInterface();
            }
        }

        private void TableauBord_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TableauBordAdmin());
        }

        private void Deconnexion_Click(object sender, RoutedEventArgs e)
        {
            UserSession.Logout();
            NavigationService.Navigate(new FormulaireConnexion());
        }

        private async void Sauvegarder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValiderFormulaire())
                {
                    return;
                }

                // Vérifier qu'une catégorie est sélectionnée
                if (CmbCategorie.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez sélectionner une catégorie.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var dto = new CreateFilmDTO
                {
                    Titre = TxtTitre.Text.Trim(),
                    Description = TxtDescription.Text.Trim(),
                    Categorie = (CmbCategorie.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "",
                    Duree = int.Parse(TxtDuree.Text.Trim()),
                    Annee = int.Parse(TxtAnnee.Text.Trim()),
                    Realisateur = TxtRealisateur.Text.Trim(),
                    Acteurs = TxtActeurs.Text.Trim(),
                    PrixAchat = decimal.Parse(TxtPrixAchat.Text.Trim()),
                    PrixLocation = decimal.Parse(TxtPrixLocation.Text.Trim()),
                    CheminAffiche = TxtCheminAffiche.Text.Trim()
                };

                // Créer un scope pour isoler cette opération
                using (var scope = ServiceLocator.CreateScope())
                {
                    var ajouterFilmUseCase = scope.ServiceProvider.GetRequiredService<AjouterFilmUseCase>();
                    var modifierFilmUseCase = scope.ServiceProvider.GetRequiredService<ModifierFilmUseCase>();

                    if (_filmIdEnEdition.HasValue)
                    {
                        // Modifier un film existant
                        await modifierFilmUseCase.ExecuteAsync(_filmIdEnEdition.Value, dto);
                        MessageBox.Show("Film modifié avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        // Ajouter un nouveau film
                        var filmAjoute = await ajouterFilmUseCase.ExecuteAsync(dto);
                        MessageBox.Show($"Film '{filmAjoute.Titre}' ajouté avec succès (ID: {filmAjoute.Id}).", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        ViderFormulaire();
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Erreur de format dans les champs numériques. Vérifiez que les valeurs sont correctes.", "Erreur de validation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Erreur de validation : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Log détaillé pour le débogage
                System.Diagnostics.Debug.WriteLine($"Erreur dans Sauvegarder_Click: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Type d'exception: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception interne: {ex.InnerException.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace interne: {ex.InnerException.StackTrace}");
                }

                var messageErreur = $"Erreur lors de l'opération : {ex.Message}";
                if (ex.InnerException != null)
                {
                    messageErreur += $"\n\nDétails : {ex.InnerException.Message}";
                }
                
                MessageBox.Show(messageErreur, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ChargerListeFilmsAsync()
        {
            try
            {
                using (var scope = ServiceLocator.CreateScope())
                {
                    var rechercherFilmsUseCase = scope.ServiceProvider.GetRequiredService<RechercherFilmsUseCase>();
                    var films = await rechercherFilmsUseCase.ExecuteAsync(null);

                    CmbFilmsExistants.Items.Clear();
                    
                    // Ajouter l'option "Nouveau film"
                    var nouveauFilmItem = new ComboBoxItem
                    {
                        Content = "-- Nouveau film --",
                        Tag = -1
                    };
                    CmbFilmsExistants.Items.Add(nouveauFilmItem);

                    // Ajouter tous les films
                    foreach (var film in films.OrderBy(f => f.Titre))
                    {
                        var item = new ComboBoxItem
                        {
                            Content = $"{film.Titre} ({film.Annee})",
                            Tag = film.Id
                        };
                        CmbFilmsExistants.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de la liste des films : {ex.Message}", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ChargerFilmAsync(int filmId)
        {
            try
            {
                using (var scope = ServiceLocator.CreateScope())
                {
                    var consulterFilmUseCase = scope.ServiceProvider.GetRequiredService<ConsulterFilmUseCase>();
                    var film = await consulterFilmUseCase.ExecuteAsync(filmId);

                    if (film != null)
                    {
                        _filmIdEnEdition = film.Id;
                        RemplirFormulaire(film);
                        
                        // Sélectionner le film dans le ComboBox
                        foreach (ComboBoxItem item in CmbFilmsExistants.Items)
                        {
                            if (item.Tag is int id && id == film.Id)
                            {
                                CmbFilmsExistants.SelectedItem = item;
                                break;
                            }
                        }
                        
                        MettreAJourInterface();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du film : {ex.Message}", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CmbFilmsExistants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbFilmsExistants.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is int filmId)
            {
                if (filmId == -1)
                {
                    // Mode "Nouveau film"
                    _filmIdEnEdition = null;
                    ViderFormulaire();
                }
                else
                {
                    // Charger le film sélectionné
                    _ = ChargerFilmAsync(filmId);
                }
                MettreAJourInterface();
            }
        }

        private void ActualiserListe_Click(object sender, RoutedEventArgs e)
        {
            _ = ChargerListeFilmsAsync();
        }

        private void RemplirFormulaire(FilmDTO film)
        {
            TxtTitre.Text = film.Titre;
            TxtAnnee.Text = film.Annee.ToString();
            TxtDuree.Text = film.Duree.ToString();
            TxtDescription.Text = film.Description;
            TxtRealisateur.Text = film.Realisateur;
            TxtActeurs.Text = film.Acteurs;
            TxtCheminAffiche.Text = film.CheminAffiche;
            TxtPrixAchat.Text = film.PrixAchat.ToString("F2");
            TxtPrixLocation.Text = film.PrixLocation.ToString("F2");

            // Sélectionner la catégorie dans le ComboBox
            foreach (ComboBoxItem item in CmbCategorie.Items)
            {
                if (item.Content?.ToString() == film.Categorie)
                {
                    CmbCategorie.SelectedItem = item;
                    break;
                }
            }
        }

        private void MettreAJourInterface()
        {
            if (_filmIdEnEdition.HasValue)
            {
                BtnSauvegarder.Content = "Modifier le Film";
                BtnSupprimer.IsEnabled = true;
            }
            else
            {
                BtnSauvegarder.Content = "Ajouter le Film";
                BtnSupprimer.IsEnabled = false;
            }
        }

        private void AjouterNouveau_Click(object sender, RoutedEventArgs e)
        {
            _filmIdEnEdition = null;
            ViderFormulaire();
            CmbFilmsExistants.SelectedIndex = 0; // Sélectionner "Nouveau film"
            MettreAJourInterface();
        }

        private async void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            if (!_filmIdEnEdition.HasValue)
            {
                MessageBox.Show("Aucun film sélectionné pour suppression.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                "Êtes-vous sûr de vouloir supprimer ce film ?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Créer un scope pour isoler cette opération
                    using (var scope = ServiceLocator.CreateScope())
                    {
                        var supprimerFilmUseCase = scope.ServiceProvider.GetRequiredService<SupprimerFilmUseCase>();
                        await supprimerFilmUseCase.ExecuteAsync(_filmIdEnEdition.Value);
                        MessageBox.Show("Film supprimé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        // Recharger la liste et passer en mode "Nouveau film"
                        await ChargerListeFilmsAsync();
                        ViderFormulaire();
                        _filmIdEnEdition = null;
                        CmbFilmsExistants.SelectedIndex = 0;
                        MettreAJourInterface();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool ValiderFormulaire()
        {
            if (string.IsNullOrWhiteSpace(TxtTitre.Text))
            {
                MessageBox.Show("Le titre est requis.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(TxtDuree.Text, out _))
            {
                MessageBox.Show("La durée doit être un nombre entier.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(TxtAnnee.Text, out _))
            {
                MessageBox.Show("L'année doit être un nombre entier.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(TxtPrixAchat.Text, out _))
            {
                MessageBox.Show("Le prix d'achat doit être un nombre décimal.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(TxtPrixLocation.Text, out _))
            {
                MessageBox.Show("Le prix de location doit être un nombre décimal.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void ViderFormulaire()
        {
            TxtTitre.Text = "";
            TxtDescription.Text = "";
            TxtDuree.Text = "";
            TxtAnnee.Text = "";
            TxtRealisateur.Text = "";
            TxtActeurs.Text = "";
            TxtCheminAffiche.Text = "";
            TxtPrixAchat.Text = "";
            TxtPrixLocation.Text = "";
            CmbCategorie.SelectedIndex = -1;
        }
    }
}
