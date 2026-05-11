using KasomaFlix.Domain.Entities;

namespace KasomaFlix.Domain.Interfaces
{
    /// <summary>
    /// Interface pour le repository des membres
    /// </summary>
    public interface IMembreRepository
    {
        Task<Membre?> GetByIdAsync(int id);
        Task<Membre?> GetByCourrielAsync(string courriel);
        Task<IEnumerable<Membre>> GetAllAsync();
        Task<Membre> AddAsync(Membre membre);
        Task UpdateAsync(Membre membre);
        Task DeleteAsync(int id);
        Task<bool> ExistsByCourrielAsync(string courriel);
    }
}

