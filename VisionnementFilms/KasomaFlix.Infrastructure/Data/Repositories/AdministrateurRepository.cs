using Microsoft.EntityFrameworkCore;
using KasomaFlix.Domain.Entities;
using KasomaFlix.Domain.Interfaces;

namespace KasomaFlix.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Implémentation du repository pour les administrateurs
    /// </summary>
    public class AdministrateurRepository : IAdministrateurRepository
    {
        private readonly VisionnementFilmsDbContext _context;

        public AdministrateurRepository(VisionnementFilmsDbContext context)
        {
            _context = context;
        }

        public async Task<Administrateur?> GetByIdAsync(int id)
        {
            return await _context.Administrateurs
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Administrateur?> GetByNomUtilisateurAsync(string nomUtilisateur)
        {
            return await _context.Administrateurs
                .FirstOrDefaultAsync(a => a.NomUtilisateur == nomUtilisateur && a.EstActif);
        }

        public async Task<Administrateur?> GetByCourrielAsync(string courriel)
        {
            return await _context.Administrateurs
                .FirstOrDefaultAsync(a => a.Courriel == courriel && a.EstActif);
        }

        public async Task<IEnumerable<Administrateur>> GetAllAsync()
        {
            return await _context.Administrateurs
                .Where(a => a.EstActif)
                .ToListAsync();
        }

        public async Task<Administrateur> AddAsync(Administrateur administrateur)
        {
            _context.Administrateurs.Add(administrateur);
            await _context.SaveChangesAsync();
            return administrateur;
        }

        public async Task UpdateAsync(Administrateur administrateur)
        {
            _context.Administrateurs.Update(administrateur);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var admin = await GetByIdAsync(id);
            if (admin != null)
            {
                // Soft delete
                admin.EstActif = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}

