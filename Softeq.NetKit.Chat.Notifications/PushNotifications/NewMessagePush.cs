using System;
using Newtonsoft.Json;

namespace Softeq.NetKit.Chat.Notifications.PushNotifications
{
    public class NewMessagePush : BasePushNotificationModel
    {
        public NewMessagePush()
        {
            Title = "New chat message.";
            Body = "New chat message. Check it out!";
            NotificationType = PushNotificationType.ChatMessage;
        }

        [JsonProperty("channelId")]
        public Guid ChannelId { get; set; }

        public override string GetData()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
