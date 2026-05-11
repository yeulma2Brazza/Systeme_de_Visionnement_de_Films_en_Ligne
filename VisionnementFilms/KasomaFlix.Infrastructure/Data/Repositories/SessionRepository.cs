using Microsoft.EntityFrameworkCore;
using KasomaFlix.Domain.Entities;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Infrastructure.Data.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly VisionnementFilmsDbContext _context;

        public SessionRepository(VisionnementFilmsDbContext context)
        {
            _context = context;
        }

        public async Task<Session?> GetByIdAsync(int id)
        {
            return await _context.Sessions
                .Include(s => s.Membre)
                .Include(s => s.Film)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Session>> GetByMembreIdAsync(int membreId)
        {
            return await _context.Sessions
                .Include(s => s.Film)
                .Where(s => s.MembreId == membreId)
                .OrderByDescending(s => s.DateDebut)
                .ToListAsync();
        }

        public async Task<Session> AddAsync(Session session)
        {
            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task UpdateAsync(Session session)
        {
            _context.Sessions.Update(session);
            await _context.SaveChangesAsync();
        }
    }
}

