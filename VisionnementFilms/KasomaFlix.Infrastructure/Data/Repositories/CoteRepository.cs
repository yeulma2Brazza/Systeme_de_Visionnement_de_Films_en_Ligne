using Microsoft.EntityFrameworkCore;
using KasomaFlix.Domain.Entities;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Infrastructure.Data.Repositories
{
    public class CoteRepository : ICoteRepository
    {
        private readonly VisionnementFilmsDbContext _context;

        public CoteRepository(VisionnementFilmsDbContext context)
        {
            _context = context;
        }

        public async Task<Cote?> GetByIdAsync(int id)
        {
            return await _context.Cotes
                .Include(c => c.Membre)
                .Include(c => c.Film)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Cote>> GetByFilmIdAsync(int filmId)
        {
            return await _context.Cotes
                .Include(c => c.Membre)
                .Where(c => c.FilmId == filmId)
                .OrderByDescending(c => c.DateCote)
                .ToListAsync();
        }

        public async Task<Cote?> GetByMembreAndFilmAsync(int membreId, int filmId)
        {
            return await _context.Cotes
                .FirstOrDefaultAsync(c => c.MembreId == membreId && c.FilmId == filmId);
        }

        public async Task<Cote> AddAsync(Cote cote)
        {
            _context.Cotes.Add(cote);
            await _context.SaveChangesAsync();
            return cote;
        }

        public async Task UpdateAsync(Cote cote)
        {
            _context.Cotes.Update(cote);
            await _context.SaveChangesAsync();
        }
    }
}

