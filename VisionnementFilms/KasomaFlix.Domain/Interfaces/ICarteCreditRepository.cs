using KasomaFlix.Domain.Entities;

namespace KasomaFlix.Domain.Interfaces
{
    /// <summary>
    /// Interface pour le repository des cartes de crédit
    /// </summary>
    public interface ICarteCreditRepository
    {
        Task<CarteCredit?> GetByIdAsync(int id);
        Task<IEnumerable<CarteCredit>> GetByMembreIdAsync(int membreId);
        Task<CarteCredit> AddAsync(CarteCredit carteCredit);
        Task UpdateAsync(CarteCredit carteCredit);
        Task DeleteAsync(int id);
        Task<CarteCredit?> GetParDefautAsync(int membreId);
    }
}
