using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Softeq.NetKit.Chat.Notifications.PushNotifications.Model
{
    public class GcmNotificationData
    {
        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("sound")]
        public string Sound { get; set; } = "default";
    }
}
