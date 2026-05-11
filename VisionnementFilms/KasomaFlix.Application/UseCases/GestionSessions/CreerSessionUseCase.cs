using KasomaFlix.Domain.Entities;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Application.UseCases.GestionSessions
{
    /// <summary>
    /// Cas d'utilisation : Créer une session de visionnement
    /// </summary>
    public class CreerSessionUseCase
    {
        private readonly ISessionRepository _sessionRepository;

        public CreerSessionUseCase(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<int> ExecuteAsync(int membreId, int filmId)
        {
            var session = new Session
            {
                MembreId = membreId,
                FilmId = filmId,
                DateDebut = DateTime.Now,
                TempsVisionne = 0
            };

            var sessionCreee = await _sessionRepository.AddAsync(session);
            return sessionCreee.Id;
        }
    }
}
