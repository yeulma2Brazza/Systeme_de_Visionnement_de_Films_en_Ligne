using KasomaFlix.Application.DTOs;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Application.UseCases.ConsultationFilm
{
    /// <summary>
    /// Cas d'utilisation : Consulter les détails d'un film
    /// </summary>
    public class ConsulterFilmUseCase
    {
        private readonly IFilmRepository _filmRepository;

        public ConsulterFilmUseCase(IFilmRepository filmRepository)
        {
            _filmRepository = filmRepository;
        }

        public async Task<FilmDTO?> ExecuteAsync(int filmId)
        {
            var film = await _filmRepository.GetByIdAsync(filmId);
            
            if (film == null || !film.EstDisponible)
            {
                return null;
            }

            return new FilmDTO
            {
                Id = film.Id,
                Titre = film.Titre,
                Description = film.Description,
                Categorie = film.Categorie,
                Duree = film.Duree,
                Annee = film.Annee,
                NoteMoyenne = film.NoteMoyenne,
                NombreVotes = film.NombreVotes,
                Realisateur = film.Realisateur,
                Acteurs = film.Acteurs,
                PrixAchat = film.PrixAchat,
                PrixLocation = film.PrixLocation,
                CheminAffiche = film.CheminAffiche
            };
        }
    }
}

