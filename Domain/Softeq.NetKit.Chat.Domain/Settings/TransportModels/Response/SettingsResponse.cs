// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Settings.TransportModels.Response
{
    public class SettingsResponse
    {
        public Guid Id { get; set; }
        public string RawSettings { get; set; }
        public Guid ChannelId { get; set; }
    }
}