// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.Message.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.Services.App.Configuration;
using Softeq.NetKit.Chat.Domain.Services.Member;

namespace Softeq.NetKit.Chat.Domain.Services.Message
{
    internal static class MessageMapper
    {
        public static MessageResponse ToMessageResponse(this Domain.Message.Message message, Domain.Message.Message lastReadMessage, CloudStorageConfiguration configuration)
        {
            var messageResponse = new MessageResponse();
            if (message != null)
            {
                messageResponse.Id = message.Id;
                messageResponse.Created = message.Created;
                messageResponse.Body = message.Body;
                messageResponse.ImageUrl = message.ImageUrl;
                messageResponse.ChannelId = message.ChannelId;
                messageResponse.Type = message.Type;
                messageResponse.Updated = message.Updated;
                messageResponse.Sender = message.Owner.ToMemberSummary(configuration);
                messageResponse.IsRead = lastReadMessage != null && message.Created <= lastReadMessage.Created;
            }
            return messageResponse;

        }
    }
}