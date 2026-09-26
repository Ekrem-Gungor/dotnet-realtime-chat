using RealtimeChat.Api.Hubs.Presence;

namespace RealtimeChat.Tests.Api
{
    public sealed class UserConnectionTrackerTests
    {
        [Fact]
        public void RegisterConnection_FirstConnection_ReturnsTrue()
        {
            UserConnectionTracker tracker = new();

            bool isFirstConnection = tracker.RegisterConnection(
                4,
                "connection-1");

            Assert.True(isFirstConnection);
        }

        [Fact]
        public void RegisterConnection_AdditionalConnection_ReturnsFalse()
        {
            UserConnectionTracker tracker = new();

            tracker.RegisterConnection(4, "connection-1");

            bool isFirstConnection = tracker.RegisterConnection(
                4,
                "connection-2");

            Assert.False(isFirstConnection);
        }

        [Fact]
        public void UnregisterConnection_WhenAnotherConnectionExists_ReturnsFalse()
        {
            UserConnectionTracker tracker = new();

            tracker.RegisterConnection(4, "connection-1");
            tracker.RegisterConnection(4, "connection-2");

            bool isLastConnection = tracker.UnregisterConnection(
                4,
                "connection-1");

            Assert.False(isLastConnection);
        }

        [Fact]
        public void UnregisterConnection_LastConnection_ReturnsTrue()
        {
            UserConnectionTracker tracker = new();

            tracker.RegisterConnection(4, "connection-1");
            tracker.RegisterConnection(4, "connection-2");

            tracker.UnregisterConnection(4, "connection-1");

            bool isLastConnection = tracker.UnregisterConnection(
                4,
                "connection-2");

            Assert.True(isLastConnection);
        }

        [Fact]
        public void RegisterConnection_DuplicateConnection_DoesNotIncreaseConnectionCount()
        {
            UserConnectionTracker tracker = new();

            bool firstRegistration = tracker.RegisterConnection(
                4,
                "connection-1");

            bool duplicateRegistration = tracker.RegisterConnection(
                4,
                "connection-1");

            bool isLastConnection = tracker.UnregisterConnection(
                4,
                "connection-1");

            Assert.True(firstRegistration);
            Assert.False(duplicateRegistration);
            Assert.True(isLastConnection);
        }

        [Fact]
        public void UnregisterConnection_UnknownConnection_ReturnsFalse()
        {
            UserConnectionTracker tracker = new();

            tracker.RegisterConnection(4, "connection-1");

            bool isLastConnection = tracker.UnregisterConnection(
                4,
                "unknown-connection");

            Assert.False(isLastConnection);
        }

        [Fact]
        public void Tracker_TracksDifferentUsersIndependently()
        {
            UserConnectionTracker tracker = new();

            bool firstUserConnection = tracker.RegisterConnection(
                4,
                "connection-1");

            bool secondUserConnection = tracker.RegisterConnection(
                8,
                "connection-2");

            Assert.True(firstUserConnection);
            Assert.True(secondUserConnection);
        }
    }
}