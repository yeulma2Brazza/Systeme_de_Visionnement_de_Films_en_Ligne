using Microsoft.EntityFrameworkCore;
using KasomaFlix.Domain.Entities;

namespace KasomaFlix.Infrastructure.Data
{
    /// <summary>
    /// Contexte Entity Framework pour la base de données
    /// </summary>
    public class VisionnementFilmsDbContext : DbContext
    {
        public VisionnementFilmsDbContext(DbContextOptions<VisionnementFilmsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Membre> Membres { get; set; }
        public DbSet<Film> Films { get; set; }
        public DbSet<Abonnement> Abonnements { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Cote> Cotes { get; set; }
        public DbSet<Administrateur> Administrateurs { get; set; }
        public DbSet<CarteCredit> CartesCredit { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Appliquer toutes les configurations depuis l'assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(VisionnementFilmsDbContext).Assembly);
        }
    }
}

