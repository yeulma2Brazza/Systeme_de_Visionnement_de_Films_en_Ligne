using KasomaFlix.Application.DTOs;
using KasomaFlix.Domain.Entities;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Application.UseCases.GestionAdmin
{
    /// <summary>
    /// Cas d'utilisation : Ajouter un film acheté (Admin)
    /// </summary>
    public class AjouterFilmUseCase
    {
        private readonly IFilmRepository _filmRepository;

        public AjouterFilmUseCase(IFilmRepository filmRepository)
        {
            _filmRepository = filmRepository;
        }

        public async Task<FilmDTO> ExecuteAsync(CreateFilmDTO dto)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(dto.Titre))
            {
                throw new ArgumentException("Le titre du film est requis.");
            }

            if (string.IsNullOrWhiteSpace(dto.Categorie))
            {
                throw new ArgumentException("La catégorie du film est requise.");
            }

            try
            {
                var film = new Film
                {
                    Titre = dto.Titre,
                    Description = dto.Description ?? string.Empty,
                    Categorie = dto.Categorie,
                    Duree = dto.Duree,
                    Annee = dto.Annee,
                    Realisateur = dto.Realisateur ?? string.Empty,
                    Acteurs = dto.Acteurs ?? string.Empty,
                    PrixAchat = dto.PrixAchat,
                    PrixLocation = dto.PrixLocation,
                    CheminAffiche = dto.CheminAffiche ?? string.Empty,
                    NoteMoyenne = 0,
                    NombreVotes = 0,
                    EstDisponible = true,
                    DateAjout = DateTime.Now
                };

                System.Diagnostics.Debug.WriteLine($"Tentative d'ajout du film: {film.Titre}");

                var filmCree = await _filmRepository.AddAsync(film);

                System.Diagnostics.Debug.WriteLine($"Film ajouté avec succès. ID: {filmCree.Id}, Titre: {filmCree.Titre}");

                return new FilmDTO
                {
                    Id = filmCree.Id,
                    Titre = filmCree.Titre,
                    Description = filmCree.Description,
                    Categorie = filmCree.Categorie,
                    Duree = filmCree.Duree,
                    Annee = filmCree.Annee,
                    NoteMoyenne = filmCree.NoteMoyenne,
                    NombreVotes = filmCree.NombreVotes,
                    Realisateur = filmCree.Realisateur,
                    Acteurs = filmCree.Acteurs,
                    PrixAchat = filmCree.PrixAchat,
                    PrixLocation = filmCree.PrixLocation,
                    CheminAffiche = filmCree.CheminAffiche
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans AjouterFilmUseCase pour '{dto.Titre}': {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception interne: {ex.InnerException.Message}");
                }
                throw;
            }
        }
    }
}

