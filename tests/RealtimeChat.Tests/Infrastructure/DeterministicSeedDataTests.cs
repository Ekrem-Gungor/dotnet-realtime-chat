using Microsoft.EntityFrameworkCore;
using RealtimeChat.Persistence.ContextClasses;

namespace RealtimeChat.Tests.Infrastructure;

public sealed class DeterministicSeedDataTests
{
    [Fact]
    public void Model_MatchesMigrationSnapshot()
    {
        DbContextOptions<RealtimeChatDbContext> options =
            new DbContextOptionsBuilder<RealtimeChatDbContext>()
                .UseSqlServer("Server=localhost;Database=ModelOnly;TrustServerCertificate=True;")
                .Options;

        using RealtimeChatDbContext context = new(options);

        Assert.False(context.Database.HasPendingModelChanges());
    }
}
