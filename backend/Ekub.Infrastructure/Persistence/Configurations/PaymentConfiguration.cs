using Ekub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekub.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasOne(payment => payment.Member)
            .WithMany(member => member.Payments)
            .HasForeignKey(payment => payment.MemberId);

        builder.HasOne(payment => payment.RecordedByMember)
            .WithMany()
            .HasForeignKey(payment => payment.RecordedBy);
    }
}
