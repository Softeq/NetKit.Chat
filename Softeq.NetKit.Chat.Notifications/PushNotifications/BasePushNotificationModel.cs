using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Softeq.NetKit.Chat.Notifications.PushNotifications
{
    public class BasePushNotificationModel : IPushNotificationModel
    {
        [JsonIgnore]
        public PushNotificationType NotificationType { get; set; }

        [JsonIgnore]
        public string Title { get; set; } = string.Empty;

        [JsonIgnore]
        public string Body { get; set; } = string.Empty;

        [JsonIgnore]
        public int Badge { get; set; } = 0;

        [JsonIgnore]
        public string Sound { get; set; } = "default";

        public virtual string GetData()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
