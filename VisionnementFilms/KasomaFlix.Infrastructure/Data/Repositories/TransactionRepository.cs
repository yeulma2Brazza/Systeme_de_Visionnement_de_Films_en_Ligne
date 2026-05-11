using Microsoft.EntityFrameworkCore;
using KasomaFlix.Domain.Entities;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Infrastructure.Data.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly VisionnementFilmsDbContext _context;

        public TransactionRepository(VisionnementFilmsDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.Membre)
                .Include(t => t.Film)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Transaction>> GetByMembreIdAsync(int membreId)
        {
            return await _context.Transactions
                .Include(t => t.Film)
                .Where(t => t.MembreId == membreId)
                .OrderByDescending(t => t.DateTransaction)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _context.Transactions
                .Include(t => t.Membre)
                .Include(t => t.Film)
                .OrderByDescending(t => t.DateTransaction)
                .ToListAsync();
        }

        public async Task<Transaction> AddAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }
    }
}

