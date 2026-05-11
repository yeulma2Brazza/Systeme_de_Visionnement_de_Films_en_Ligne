using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KasomaFlix.Domain.Entities;

namespace KasomaFlix.Infrastructure.Data.Configurations
{
    public class CoteConfiguration : IEntityTypeConfiguration<Cote>
    {
        public void Configure(EntityTypeBuilder<Cote> builder)
        {
            builder.ToTable("Cotes", t => t.HasCheckConstraint("CK_Cote_Note", "[Note] >= 1 AND [Note] <= 5"));

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Note)
                .IsRequired();

            builder.Property(c => c.Commentaire)
                .HasMaxLength(1000);

            // Un membre ne peut coter un film qu'une seule fois
            builder.HasIndex(c => new { c.MembreId, c.FilmId })
                .IsUnique();
        }
    }
}

