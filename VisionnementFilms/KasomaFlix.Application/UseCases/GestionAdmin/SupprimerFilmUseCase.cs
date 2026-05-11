using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Application.UseCases.GestionAdmin
{
    /// <summary>
    /// Cas d'utilisation : Enlever un film (Admin)
    /// </summary>
    public class SupprimerFilmUseCase
    {
        private readonly IFilmRepository _filmRepository;

        public SupprimerFilmUseCase(IFilmRepository filmRepository)
        {
            _filmRepository = filmRepository;
        }

        public async Task<bool> ExecuteAsync(int filmId)
        {
            var film = await _filmRepository.GetByIdAsync(filmId);
            if (film == null)
            {
                throw new ArgumentException($"Le film avec l'ID {filmId} n'existe pas.");
            }

            await _filmRepository.DeleteAsync(filmId);
            return true;
        }
    }
}

