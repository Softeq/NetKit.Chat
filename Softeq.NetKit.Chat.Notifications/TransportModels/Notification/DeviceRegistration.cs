using System;
using System.Collections.Generic;
using System.Text;

namespace Softeq.NetKit.Chat.Notifications.TransportModels.Notification
{
    public class DeviceRegistration
    {
        public DevicePlatform Platform { get; set; }
        public string DeviceToken { get; set; }
        public string[] Tags { get; set; }
    }
}
