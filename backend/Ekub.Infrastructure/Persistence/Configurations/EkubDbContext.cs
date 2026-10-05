using Microsoft.EntityFrameworkCore;
using Ekub.Domain.Entities;

namespace Ekub.Infrastructure.Data;

public class EkubDbContext(DbContextOptions<EkubDbContext> options) : DbContext(options)
{
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Circle> Circles => Set<Circle>();
    public DbSet<CircleMember> CircleMembers => Set<CircleMember>();
    public DbSet<CircleBankAccount> CircleBankAccounts => Set<CircleBankAccount>();
    public DbSet<Round> Rounds => Set<Round>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Payout> Payouts => Set<Payout>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(EkubDbContext).Assembly);
    }
}