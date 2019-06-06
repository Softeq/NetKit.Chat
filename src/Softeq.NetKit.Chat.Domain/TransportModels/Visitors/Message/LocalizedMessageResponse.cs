// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Client.SDK.Models.Visitors.Localization;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Localization;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Visitors.Message
{
    public abstract class LocalizedMessageResponse<T> where T: class, new()
    {
        public LocalizationResponse Localization { get; set; }

        public abstract void Accept(ILocalizationVisitor<T> visitor);
    }
}
