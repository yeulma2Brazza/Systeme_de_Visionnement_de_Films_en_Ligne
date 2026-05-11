using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;
using KasomaFlix.Application.UseCases.GestionSessions;
using KasomaFlix.Presentation;
using KasomaFlix.Presentation.Services;

namespace KasomaFlix.Presentation.Views
{
    public partial class LecteurVideo : Page
    {
        private readonly CreerSessionUseCase _creerSessionUseCase;
        private readonly TerminerSessionUseCase _terminerSessionUseCase;
        private int _filmId;
        private int? _sessionId;
        private DateTime _debutLecture;
        private bool _lectureEnCours = false;

        public LecteurVideo(int filmId)
        {
            InitializeComponent();
            _filmId = filmId;
            _creerSessionUseCase = ServiceLocator.GetService<CreerSessionUseCase>();
            _terminerSessionUseCase = ServiceLocator.GetService<TerminerSessionUseCase>();
            Loaded += LecteurVideo_Loaded;
        }

        private async void LecteurVideo_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserSession.IsLoggedIn() && UserSession.GetUserId().HasValue)
            {
                try
                {
                    _sessionId = await _creerSessionUseCase.ExecuteAsync(UserSession.GetUserId().Value, _filmId);
                    _debutLecture = DateTime.Now;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la création de la session : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            _lectureEnCours = true;
            // VideoPlayer.Play();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            // VideoPlayer.Pause();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _lectureEnCours = false;
            // VideoPlayer.Stop();
        }

        private async void FermerLecteur_Click(object sender, RoutedEventArgs e)
        {
            if (_sessionId.HasValue)
            {
                try
                {
                    var tempsVisionne = (int)(DateTime.Now - _debutLecture).TotalSeconds;
                    // Créer un scope pour isoler cette opération
                    using (var scope = ServiceLocator.CreateScope())
                    {
                        var terminerSessionUseCase = scope.ServiceProvider.GetRequiredService<TerminerSessionUseCase>();
                        await terminerSessionUseCase.ExecuteAsync(_sessionId.Value, tempsVisionne);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la fermeture de la session : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            NavigationService.Navigate(new DetailsFilm(_filmId));
        }

        private async void CoterFilm_Click(object sender, RoutedEventArgs e)
        {
            if (_sessionId.HasValue)
            {
                try
                {
                    var tempsVisionne = (int)(DateTime.Now - _debutLecture).TotalSeconds;
                    // Créer un scope pour isoler cette opération
                    using (var scope = ServiceLocator.CreateScope())
                    {
                        var terminerSessionUseCase = scope.ServiceProvider.GetRequiredService<TerminerSessionUseCase>();
                        await terminerSessionUseCase.ExecuteAsync(_sessionId.Value, tempsVisionne);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la fermeture de la session : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            NavigationService.Navigate(new DetailsFilm(_filmId));
        }
    }
}
