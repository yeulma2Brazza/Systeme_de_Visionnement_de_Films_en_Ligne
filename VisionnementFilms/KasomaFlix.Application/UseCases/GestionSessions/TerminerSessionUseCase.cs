using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Application.UseCases.GestionSessions
{
    /// <summary>
    /// Cas d'utilisation : Terminer une session de visionnement
    /// </summary>
    public class TerminerSessionUseCase
    {
        private readonly ISessionRepository _sessionRepository;

        public TerminerSessionUseCase(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task ExecuteAsync(int sessionId, int tempsVisionne)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session != null)
            {
                session.DateFin = DateTime.Now;
                session.TempsVisionne = tempsVisionne;
                await _sessionRepository.UpdateAsync(session);
            }
        }
    }
}
