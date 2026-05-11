using KasomaFlix.Domain.Entities;

namespace KasomaFlix.Domain.Interfaces
{
    /// <summary>
    /// Interface pour le repository des cotes
    /// </summary>
    public interface ICoteRepository
    {
        Task<Cote?> GetByIdAsync(int id);
        Task<IEnumerable<Cote>> GetByFilmIdAsync(int filmId);
        Task<Cote?> GetByMembreAndFilmAsync(int membreId, int filmId);
        Task<Cote> AddAsync(Cote cote);
        Task UpdateAsync(Cote cote);
    }
}

