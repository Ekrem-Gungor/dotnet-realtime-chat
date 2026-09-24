using Microsoft.EntityFrameworkCore;
using RealtimeChat.Domain.Entities.Concretes;
using RealtimeChat.Persistence.ContextClasses;

namespace RealtimeChat.Tests.Infrastructure;

public sealed class DeterministicSeedDataTests
{
    [Fact]
    public void Model_UsesMigrationSnapshotDatesForSeedData()
    {
        DbContextOptions<RealtimeChatDbContext> options =
            new DbContextOptionsBuilder<RealtimeChatDbContext>()
                .UseSqlServer("Server=localhost;Database=ModelOnly;TrustServerCertificate=True;")
                .Options;

        using RealtimeChatDbContext context = new(options);

        Assert.False(context.Database.HasPendingModelChanges());

        Assert.Equal(
            [SeedDate(276, 5318), SeedDate(276, 5402), SeedDate(276, 5406)],
            SeedDates<AppRole>(context));
        Assert.Equal(
            [SeedDate(276, 5606), SeedDate(340, 8462), SeedDate(405, 1029)],
            SeedDates<AppUser>(context));
        Assert.Equal(
            [SeedDate(276, 5578), SeedDate(276, 5580), SeedDate(276, 5581)],
            SeedDates<AppUserProfile>(context));
        Assert.Equal(
            [SeedDate(470, 4265), SeedDate(470, 4282), SeedDate(470, 4283)],
            SeedDates<AppUserRole>(context));

        var users = context.Model
            .FindEntityType(typeof(AppUser))!
            .GetSeedData()
            .OrderBy(seed => (int)seed[nameof(AppUser.Id)]!)
            .ToArray();

        Assert.Equal(
            ["seed-user-developer", "seed-user-admin", "seed-user-member"],
            users.Select(seed => (string)seed[nameof(AppUser.ConcurrencyStamp)]!).ToArray());
        Assert.Equal(
            ["seed-user-developer", "seed-user-admin", "seed-user-member"],
            users.Select(seed => (string)seed[nameof(AppUser.SecurityStamp)]!).ToArray());
    }

    private static DateTime[] SeedDates<TEntity>(RealtimeChatDbContext context)
    {
        return context.Model
            .FindEntityType(typeof(TEntity))!
            .GetSeedData()
            .Select(seed => (DateTime)seed[nameof(AppUser.CreatedDate)]!)
            .OrderBy(date => date)
            .ToArray();
    }

    private static DateTime SeedDate(int millisecond, long ticks)
    {
        return new DateTime(
            2025,
            7,
            29,
            20,
            35,
            21,
            millisecond,
            DateTimeKind.Local).AddTicks(ticks);
    }
}
