using Microsoft.EntityFrameworkCore;
using KasomaFlix.Domain.Entities;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Implémentation du repository pour les cartes de crédit
    /// </summary>
    public class CarteCreditRepository : ICarteCreditRepository
    {
        private readonly VisionnementFilmsDbContext _context;

        public CarteCreditRepository(VisionnementFilmsDbContext context)
        {
            _context = context;
        }

        public async Task<CarteCredit?> GetByIdAsync(int id)
        {
            return await _context.CartesCredit
                .Include(c => c.Membre)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<CarteCredit>> GetByMembreIdAsync(int membreId)
        {
            return await _context.CartesCredit
                .Where(c => c.MembreId == membreId)
                .OrderByDescending(c => c.EstParDefaut)
                .ThenByDescending(c => c.DateAjout)
                .ToListAsync();
        }

        public async Task<CarteCredit> AddAsync(CarteCredit carteCredit)
        {
            _context.CartesCredit.Add(carteCredit);
            await _context.SaveChangesAsync();
            await _context.Entry(carteCredit).ReloadAsync();
            return carteCredit;
        }

        public async Task UpdateAsync(CarteCredit carteCredit)
        {
            _context.CartesCredit.Update(carteCredit);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var carteCredit = await _context.CartesCredit.FindAsync(id);
            if (carteCredit != null)
            {
                _context.CartesCredit.Remove(carteCredit);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<CarteCredit?> GetParDefautAsync(int membreId)
        {
            return await _context.CartesCredit
                .FirstOrDefaultAsync(c => c.MembreId == membreId && c.EstParDefaut);
        }
    }
}
