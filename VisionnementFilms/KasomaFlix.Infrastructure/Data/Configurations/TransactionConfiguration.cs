using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KasomaFlix.Domain.Entities;

namespace KasomaFlix.Infrastructure.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.TypeTransaction)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Montant)
                .HasPrecision(10, 2);

            builder.Property(t => t.Statut)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}

