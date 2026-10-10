using Ekub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekub.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(payment => payment.BankName)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(payment => payment.TransactionId)
            .HasMaxLength(200);

        builder.HasOne(payment => payment.Member)
            .WithMany(member => member.Payments)
            .HasForeignKey(payment => payment.MemberId);

        builder.HasOne(payment => payment.RecordedByMember)
            .WithMany()
            .HasForeignKey(payment => payment.RecordedBy);

        builder.HasOne(payment => payment.ReviewedByMember)
            .WithMany()
            .HasForeignKey(payment => payment.ReviewedByMemberId);

        builder.HasIndex(payment => new { payment.RoundId, payment.MemberId })
            .IsUnique();
    }
}
