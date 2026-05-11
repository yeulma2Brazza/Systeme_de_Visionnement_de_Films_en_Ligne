using Microsoft.EntityFrameworkCore;
using KasomaFlix.Domain.Entities;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Infrastructure.Data.Repositories
{
    public class AbonnementRepository : IAbonnementRepository
    {
        private readonly VisionnementFilmsDbContext _context;

        public AbonnementRepository(VisionnementFilmsDbContext context)
        {
            _context = context;
        }

        public async Task<Abonnement?> GetByIdAsync(int id)
        {
            return await _context.Abonnements
                .Include(a => a.Membre)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Abonnement>> GetByMembreIdAsync(int membreId)
        {
            return await _context.Abonnements
                .Where(a => a.MembreId == membreId)
                .ToListAsync();
        }

        public async Task<Abonnement> AddAsync(Abonnement abonnement)
        {
            _context.Abonnements.Add(abonnement);
            await _context.SaveChangesAsync();
            return abonnement;
        }

        public async Task UpdateAsync(Abonnement abonnement)
        {
            _context.Abonnements.Update(abonnement);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var abonnement = await GetByIdAsync(id);
            if (abonnement != null)
            {
                _context.Abonnements.Remove(abonnement);
                await _context.SaveChangesAsync();
            }
        }
    }
}

