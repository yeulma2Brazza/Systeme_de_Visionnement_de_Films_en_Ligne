using KasomaFlix.Domain.Entities;

namespace KasomaFlix.Domain.Interfaces
{
    /// <summary>
    /// Interface pour le repository des films
    /// </summary>
    public interface IFilmRepository
    {
        Task<Film?> GetByIdAsync(int id);
        Task<IEnumerable<Film>> GetAllAsync();
        Task<IEnumerable<Film>> RechercherAsync(string? titre, string? categorie, int? annee);
        Task<Film> AddAsync(Film film);
        Task UpdateAsync(Film film);
        Task DeleteAsync(int id);
    }
}

