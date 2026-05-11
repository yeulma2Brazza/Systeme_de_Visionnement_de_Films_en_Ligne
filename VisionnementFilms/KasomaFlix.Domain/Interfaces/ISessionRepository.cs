using KasomaFlix.Domain.Entities;

namespace KasomaFlix.Domain.Interfaces
{
    /// <summary>
    /// Interface pour le repository des sessions
    /// </summary>
    public interface ISessionRepository
    {
        Task<Session?> GetByIdAsync(int id);
        Task<IEnumerable<Session>> GetByMembreIdAsync(int membreId);
        Task<Session> AddAsync(Session session);
        Task UpdateAsync(Session session);
    }
}

