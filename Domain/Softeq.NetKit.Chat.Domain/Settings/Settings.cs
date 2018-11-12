// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.Base;

namespace Softeq.NetKit.Chat.Domain.Settings
{
    public class Settings : IBaseEntity<Guid>
    {
        public string RawSettings { get; set; }
        public Guid ChannelId { get; set; }
        public Channel.Channel Channel { get; set; }
        public Guid Id { get; set; }
    }
}