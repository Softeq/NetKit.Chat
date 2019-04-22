// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.TransportModels.Response.Localization;
using Softeq.NetKit.Chat.Domain.TransportModels.Visitors.Localization;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Visitors.Message
{
    public abstract class LocalizedMessageResponse<T> where T: class, new()
    {
        public LocalizationResponse Localization { get; set; }

        public abstract void Accept(ILocalizationVisitor<T> visitor);
    }
}
