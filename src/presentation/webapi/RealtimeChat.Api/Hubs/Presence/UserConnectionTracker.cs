namespace RealtimeChat.Api.Hubs.Presence
{
    public sealed class UserConnectionTracker : IUserConnectionTracker
    {
        private readonly object _syncRoot = new();

        private readonly Dictionary<int, HashSet<string>> _connectionsByUser = [];

        public bool RegisterConnection(int userId, string connectionId)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(userId, 1);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);

            lock (_syncRoot)
            {
                if (!_connectionsByUser.TryGetValue(userId, out HashSet<string>? connections))
                {
                    connections = [];
                    _connectionsByUser[userId] = connections;
                }

                bool wasAdded = connections.Add(connectionId);

                return wasAdded && connections.Count == 1;
            }
        }

        public bool UnregisterConnection(int userId, string connectionId)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(userId, 1);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);

            lock (_syncRoot)
            {
                if (!_connectionsByUser.TryGetValue(
                        userId,
                        out HashSet<string>? connections))
                {
                    return false;
                }

                if (!connections.Remove(connectionId))
                {
                    return false;
                }

                if (connections.Count > 0)
                {
                    return false;
                }

                _connectionsByUser.Remove(userId);

                return true;
            }
        }
    }
}
