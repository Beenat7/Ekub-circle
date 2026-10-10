using Ekub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ekub.Infrastructure.Persistence.Configurations;

public sealed class RoundEntityConfiguration : IEntityTypeConfiguration<Round>
{
    public void Configure(EntityTypeBuilder<Round> builder)
    {
        builder.HasIndex(round => new { round.CircleId, round.RoundNumber })
            .IsUnique();
    }
}
