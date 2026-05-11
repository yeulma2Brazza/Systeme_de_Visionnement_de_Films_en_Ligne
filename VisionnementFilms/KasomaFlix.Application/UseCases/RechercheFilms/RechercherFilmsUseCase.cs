using KasomaFlix.Application.DTOs;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Application.UseCases.RechercheFilms
{
    /// <summary>
    /// Cas d'utilisation : Rechercher des films
    /// </summary>
    public class RechercherFilmsUseCase
    {
        private readonly IFilmRepository _filmRepository;

        public RechercherFilmsUseCase(IFilmRepository filmRepository)
        {
            _filmRepository = filmRepository;
        }

        public async Task<IEnumerable<FilmDTO>> ExecuteAsync(RechercheFilmsDTO? criteres = null)
        {
            IEnumerable<Domain.Entities.Film> films;

            if (criteres == null || (string.IsNullOrWhiteSpace(criteres.Titre) && 
                                     string.IsNullOrWhiteSpace(criteres.Categorie) && 
                                     !criteres.Annee.HasValue))
            {
                // Si aucun critère, retourner tous les films
                films = await _filmRepository.GetAllAsync();
            }
            else
            {
                // Recherche avec critères
                films = await _filmRepository.RechercherAsync(
                    criteres.Titre,
                    criteres.Categorie,
                    criteres.Annee
                );
            }

            return films.Select(f => new FilmDTO
            {
                Id = f.Id,
                Titre = f.Titre,
                Description = f.Description,
                Categorie = f.Categorie,
                Duree = f.Duree,
                Annee = f.Annee,
                NoteMoyenne = f.NoteMoyenne,
                NombreVotes = f.NombreVotes,
                Realisateur = f.Realisateur,
                Acteurs = f.Acteurs,
                PrixAchat = f.PrixAchat,
                PrixLocation = f.PrixLocation,
                CheminAffiche = f.CheminAffiche
            });
        }
    }
}

