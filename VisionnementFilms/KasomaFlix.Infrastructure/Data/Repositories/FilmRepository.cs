using Microsoft.EntityFrameworkCore;
using KasomaFlix.Domain.Entities;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Implémentation du repository pour les films
    /// </summary>
    public class FilmRepository : IFilmRepository
    {
        private readonly VisionnementFilmsDbContext _context;

        public FilmRepository(VisionnementFilmsDbContext context)
        {
            _context = context;
        }

        public async Task<Film?> GetByIdAsync(int id)
        {
            return await _context.Films
                .Include(f => f.Cotes)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<Film>> GetAllAsync()
        {
            return await _context.Films
                .Where(f => f.EstDisponible)
                .OrderByDescending(f => f.DateAjout)
                .ToListAsync();
        }

        public async Task<IEnumerable<Film>> RechercherAsync(string? titre, string? categorie, int? annee)
        {
            var query = _context.Films
                .Where(f => f.EstDisponible)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(titre))
            {
                query = query.Where(f => f.Titre.Contains(titre));
            }

            if (!string.IsNullOrWhiteSpace(categorie))
            {
                query = query.Where(f => f.Categorie == categorie);
            }

            if (annee.HasValue)
            {
                query = query.Where(f => f.Annee == annee.Value);
            }

            return await query
                .OrderByDescending(f => f.NoteMoyenne)
                .ThenByDescending(f => f.DateAjout)
                .ToListAsync();
        }

        public async Task<Film> AddAsync(Film film)
        {
            try
            {
                _context.Films.Add(film);
                var rowsAffected = await _context.SaveChangesAsync();
                
                // Vérifier que le film a bien été sauvegardé
                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException("Aucune ligne n'a été affectée lors de la sauvegarde du film.");
                }
                
                // Recharger le film depuis la base pour obtenir l'ID généré
                await _context.Entry(film).ReloadAsync();
                
                return film;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de l'ajout du film '{film.Titre}': {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception interne: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task UpdateAsync(Film film)
        {
            _context.Films.Update(film);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var film = await GetByIdAsync(id);
            if (film != null)
            {
                // Soft delete : marquer comme non disponible
                film.EstDisponible = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}

