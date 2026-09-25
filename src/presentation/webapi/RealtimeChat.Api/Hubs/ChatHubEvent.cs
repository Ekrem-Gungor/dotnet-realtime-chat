namespace RealtimeChat.Api.Hubs
{
    public static class ChatHubEvent
    {
        public const string ReceiveMessage = nameof(ReceiveMessage);
        public const string ReceiveSystemMessage = nameof(ReceiveSystemMessage);
        public const string ReceiveOnlineUsers = nameof(ReceiveOnlineUsers);
    }
}
