using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Notifications.PushNotifications.Model
{
    public class GcmNativeNotification : Notification
    {
        [JsonProperty("notification")]
        public GcmNotificationData Notification { get; set; }

        [JsonProperty("collapse_key")]
        public string CollapseKey { get; set; }

        [JsonProperty("time_to_live")]
        public int? TimeToLive { get; set; }

        [JsonProperty("data")]
        public dynamic Data { get; set; }
    }
}
