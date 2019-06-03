using System;
using Newtonsoft.Json;
using Softeq.NetKit.Services.PushNotifications.Models;

namespace Softeq.NetKit.Chat.Notifications.PushNotifications
{
    public class NewMessagePush : PushNotificationMessage
    {
        public NewMessagePush()
        {
            Title = "New chat message.";
            Body = "New chat message. Check it out!";
            NotificationType = (int)PushNotificationType.ChatMessage;
        }

        [JsonProperty("channelId")]
        public Guid ChannelId { get; set; }

        public override string GetData()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
