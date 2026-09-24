using StackExchange.Redis;

namespace RealtimeChat.Tests.Infrastructure;

public sealed class RedisFixture : IAsyncLifetime
{
    public IConnectionMultiplexer Connection { get; private set; } = null!;

    public IDatabase Database => Connection.GetDatabase();

    public async Task InitializeAsync()
    {
        string connectionString = Environment.GetEnvironmentVariable("TEST_REDIS_CONNECTION")
            ?? "localhost:6379,defaultDatabase=15,abortConnect=false";

        Connection = await ConnectionMultiplexer.ConnectAsync(connectionString);
        await Database.PingAsync();
    }

    public async Task DisposeAsync()
    {
        await Connection.CloseAsync();
        Connection.Dispose();
    }
}

[CollectionDefinition(RedisCollection.Name, DisableParallelization = true)]
public sealed class RedisCollection : ICollectionFixture<RedisFixture>
{
    public const string Name = "Redis";
}
