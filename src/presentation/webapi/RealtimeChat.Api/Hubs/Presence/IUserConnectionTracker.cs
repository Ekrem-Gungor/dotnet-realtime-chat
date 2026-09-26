namespace RealtimeChat.Api.Hubs.Presence
{
    public interface IUserConnectionTracker
    {
        // İlk aktif bağlantı kaydedildiğinde true döner.
        bool RegisterConnection(int userId, string connectionId);

        // Kullanıcının son aktif bağlantısı kaldırıldığında true döner.
        bool UnregisterConnection(int userId, string connectionId);
    }
}
