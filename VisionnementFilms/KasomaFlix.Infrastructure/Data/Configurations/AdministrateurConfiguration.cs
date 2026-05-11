using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KasomaFlix.Domain.Entities;

namespace KasomaFlix.Infrastructure.Data.Configurations
{
    public class AdministrateurConfiguration : IEntityTypeConfiguration<Administrateur>
    {
        public void Configure(EntityTypeBuilder<Administrateur> builder)
        {
            builder.ToTable("Administrateurs");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.NomUtilisateur)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(a => a.NomUtilisateur)
                .IsUnique();

            builder.Property(a => a.MotDePasseHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.Nom)
                .HasMaxLength(100);

            builder.Property(a => a.Prenom)
                .HasMaxLength(100);

            builder.Property(a => a.Courriel)
                .HasMaxLength(255);

            builder.HasIndex(a => a.Courriel)
                .IsUnique();
        }
    }
}

