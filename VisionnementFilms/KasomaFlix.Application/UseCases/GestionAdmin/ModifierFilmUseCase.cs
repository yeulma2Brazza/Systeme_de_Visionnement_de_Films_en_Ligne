using KasomaFlix.Application.DTOs;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Application.UseCases.GestionAdmin
{
    /// <summary>
    /// Cas d'utilisation : Modifier les informations d'un film (Admin)
    /// </summary>
    public class ModifierFilmUseCase
    {
        private readonly IFilmRepository _filmRepository;

        public ModifierFilmUseCase(IFilmRepository filmRepository)
        {
            _filmRepository = filmRepository;
        }

        public async Task<FilmDTO> ExecuteAsync(int filmId, CreateFilmDTO dto)
        {
            var film = await _filmRepository.GetByIdAsync(filmId);
            if (film == null)
            {
                throw new ArgumentException($"Le film avec l'ID {filmId} n'existe pas.");
            }

            // Mettre à jour les propriétés
            film.Titre = dto.Titre;
            film.Description = dto.Description;
            film.Categorie = dto.Categorie;
            film.Duree = dto.Duree;
            film.Annee = dto.Annee;
            film.Realisateur = dto.Realisateur;
            film.Acteurs = dto.Acteurs;
            film.PrixAchat = dto.PrixAchat;
            film.PrixLocation = dto.PrixLocation;
            film.CheminAffiche = dto.CheminAffiche;

            await _filmRepository.UpdateAsync(film);

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

