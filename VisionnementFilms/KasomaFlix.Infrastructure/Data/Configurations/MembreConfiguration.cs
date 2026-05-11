using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KasomaFlix.Domain.Entities;

namespace KasomaFlix.Infrastructure.Data.Configurations
{
    public class MembreConfiguration : IEntityTypeConfiguration<Membre>
    {
        public void Configure(EntityTypeBuilder<Membre> builder)
        {
            builder.ToTable("Membres");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Prenom)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Nom)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Courriel)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(m => m.Courriel)
                .IsUnique();

            builder.Property(m => m.MotDePasseHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(m => m.Adresse)
                .HasMaxLength(500);

            builder.Property(m => m.Telephone)
                .HasMaxLength(20);

            builder.Property(m => m.Solde)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            // Relations
            builder.HasMany(m => m.Abonnements)
                .WithOne(a => a.Membre)
                .HasForeignKey(a => a.MembreId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m => m.Transactions)
                .WithOne(t => t.Membre)
                .HasForeignKey(t => t.MembreId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(m => m.Sessions)
                .WithOne(s => s.Membre)
                .HasForeignKey(s => s.MembreId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m => m.Cotes)
                .WithOne(c => c.Membre)
                .HasForeignKey(c => c.MembreId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

