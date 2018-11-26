using System;
using System.Collections.Generic;
using System.Text;

namespace Softeq.NetKit.Chat.Notifications.TransportModels.Notification
{
    public class Entity<T>
    {
        public virtual T Id { get; set; }
    }
}
