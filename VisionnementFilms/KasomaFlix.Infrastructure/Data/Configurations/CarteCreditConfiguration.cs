using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KasomaFlix.Domain.Entities;

namespace KasomaFlix.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Configuration Entity Framework pour l'entité CarteCredit
    /// </summary>
    public class CarteCreditConfiguration : IEntityTypeConfiguration<CarteCredit>
    {
        public void Configure(EntityTypeBuilder<CarteCredit> builder)
        {
            builder.ToTable("CartesCredit");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(c => c.MembreId)
                .HasColumnName("MembreId")
                .IsRequired();

            builder.Property(c => c.TypeCarte)
                .HasColumnName("TypeCarte")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.NumeroCarte)
                .HasColumnName("NumeroCarte")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(c => c.NomTitulaire)
                .HasColumnName("NomTitulaire")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.DateExpiration)
                .HasColumnName("DateExpiration")
                .IsRequired();

            builder.Property(c => c.EmailPayPal)
                .HasColumnName("EmailPayPal")
                .HasMaxLength(255);

            builder.Property(c => c.EstParDefaut)
                .HasColumnName("EstParDefaut")
                .HasDefaultValue(false);

            builder.Property(c => c.DateAjout)
                .HasColumnName("DateAjout")
                .IsRequired();

            // Relation avec Membre
            builder.HasOne(c => c.Membre)
                .WithMany(m => m.CartesCredit)
                .HasForeignKey(c => c.MembreId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index
            builder.HasIndex(c => c.MembreId);
        }
    }
}
