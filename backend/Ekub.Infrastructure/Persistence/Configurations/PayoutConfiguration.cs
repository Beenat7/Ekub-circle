using Ekub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekub.Infrastructure.Persistence.Configurations;

public class PayoutConfiguration : IEntityTypeConfiguration<Payout>
{
    public void Configure(EntityTypeBuilder<Payout> builder)
    {
        builder.HasOne(payout => payout.Member)
            .WithMany(member => member.Payouts)
            .HasForeignKey(payout => payout.MemberId);

        builder.HasOne(payout => payout.RecordedByMember)
            .WithMany()
            .HasForeignKey(payout => payout.RecordedBy);
    }
}
