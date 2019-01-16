using EnsureThat;

namespace Softeq.NetKit.Chat.Notifications
{
    public static class PushNotificationsTagTemplates
    {
        public static string GetMemberSubscriptionTag(string userId)
        {
            Ensure.That(userId).IsNotNullOrEmpty();

            return $"user-{userId}";
        }

        public static string GetChatChannelTag(string channelId)
        {
            Ensure.That(channelId).IsNotNullOrEmpty();

            return $"chat-channel-{channelId}";
        }
    }
}
