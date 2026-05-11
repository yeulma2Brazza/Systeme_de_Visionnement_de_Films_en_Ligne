using Microsoft.EntityFrameworkCore;
using KasomaFlix.Domain.Entities;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Implémentation du repository pour les membres
    /// </summary>
    public class MembreRepository : IMembreRepository
    {
        private readonly VisionnementFilmsDbContext _context;

        public MembreRepository(VisionnementFilmsDbContext context)
        {
            _context = context;
        }

        public async Task<Membre?> GetByIdAsync(int id)
        {
            return await _context.Membres
                .Include(m => m.Abonnements)
                .Include(m => m.Transactions)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Membre?> GetByCourrielAsync(string courriel)
        {
            return await _context.Membres
                .Include(m => m.Abonnements)
                .FirstOrDefaultAsync(m => m.Courriel == courriel);
        }

        public async Task<IEnumerable<Membre>> GetAllAsync()
        {
            return await _context.Membres
                .Include(m => m.Abonnements)
                .ToListAsync();
        }

        public async Task<Membre> AddAsync(Membre membre)
        {
            _context.Membres.Add(membre);
            await _context.SaveChangesAsync();
            return membre;
        }

        public async Task UpdateAsync(Membre membre)
        {
            _context.Membres.Update(membre);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var membre = await GetByIdAsync(id);
            if (membre != null)
            {
                _context.Membres.Remove(membre);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsByCourrielAsync(string courriel)
        {
            return await _context.Membres
                .AnyAsync(m => m.Courriel == courriel);
        }
    }
}

