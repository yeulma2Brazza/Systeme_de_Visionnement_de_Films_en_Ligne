using KasomaFlix.Domain.Entities;

namespace KasomaFlix.Domain.Interfaces
{
    /// <summary>
    /// Interface pour le repository des administrateurs
    /// </summary>
    public interface IAdministrateurRepository
    {
        Task<Administrateur?> GetByIdAsync(int id);
        Task<Administrateur?> GetByNomUtilisateurAsync(string nomUtilisateur);
        Task<Administrateur?> GetByCourrielAsync(string courriel);
        Task<IEnumerable<Administrateur>> GetAllAsync();
        Task<Administrateur> AddAsync(Administrateur administrateur);
        Task UpdateAsync(Administrateur administrateur);
        Task DeleteAsync(int id);
    }
}

