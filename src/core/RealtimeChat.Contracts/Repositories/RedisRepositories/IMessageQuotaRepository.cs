namespace RealtimeChat.Contracts.Repositories.RedisRepositories;

public interface IMessageQuotaRepository
{
    Task<bool> TryConsumeAsync(string userId);
}
