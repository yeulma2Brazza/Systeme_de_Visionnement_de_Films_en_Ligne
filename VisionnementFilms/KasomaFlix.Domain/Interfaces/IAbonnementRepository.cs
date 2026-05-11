using KasomaFlix.Domain.Entities;

namespace KasomaFlix.Domain.Interfaces
{
    /// <summary>
    /// Interface pour le repository des abonnements
    /// </summary>
    public interface IAbonnementRepository
    {
        Task<Abonnement?> GetByIdAsync(int id);
        Task<IEnumerable<Abonnement>> GetByMembreIdAsync(int membreId);
        Task<Abonnement> AddAsync(Abonnement abonnement);
        Task UpdateAsync(Abonnement abonnement);
        Task DeleteAsync(int id);
    }
}

