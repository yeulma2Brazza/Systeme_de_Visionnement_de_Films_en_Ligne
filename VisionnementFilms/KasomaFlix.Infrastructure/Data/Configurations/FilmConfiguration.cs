using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KasomaFlix.Domain.Entities;

namespace KasomaFlix.Infrastructure.Data.Configurations
{
    public class FilmConfiguration : IEntityTypeConfiguration<Film>
    {
        public void Configure(EntityTypeBuilder<Film> builder)
        {
            builder.ToTable("Films");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Titre)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(f => f.Description)
                .HasMaxLength(2000);

            builder.Property(f => f.Categorie)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(f => f.Realisateur)
                .HasMaxLength(200);

            builder.Property(f => f.Acteurs)
                .HasMaxLength(500);

            builder.Property(f => f.CheminAffiche)
                .HasMaxLength(500);

            builder.Property(f => f.NoteMoyenne)
                .HasPrecision(3, 2);

            builder.Property(f => f.PrixAchat)
                .HasPrecision(10, 2);

            builder.Property(f => f.PrixLocation)
                .HasPrecision(10, 2);

            // Relations
            builder.HasMany(f => f.Transactions)
                .WithOne(t => t.Film)
                .HasForeignKey(t => t.FilmId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(f => f.Sessions)
                .WithOne(s => s.Film)
                .HasForeignKey(s => s.FilmId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(f => f.Cotes)
                .WithOne(c => c.Film)
                .HasForeignKey(c => c.FilmId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

