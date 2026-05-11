using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KasomaFlix.Domain.Entities;

namespace KasomaFlix.Infrastructure.Data.Configurations
{
    public class AbonnementConfiguration : IEntityTypeConfiguration<Abonnement>
    {
        public void Configure(EntityTypeBuilder<Abonnement> builder)
        {
            builder.ToTable("Abonnements");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.TypeAbonnement)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.Prix)
                .HasPrecision(10, 2);
        }
    }
}

