// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Data.Cloud.DataProviders;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;

namespace Softeq.NetKit.Chat.Domain.Services.Mappers
{
    internal static class MessageMapper
    {
        public static MessageResponse ToMessageResponse(this Message message, Message lastReadMessage, ICloudImageProvider cloudImageProvider)
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
                var memberAvatarUrl = cloudImageProvider.GetMemberAvatarUrl(message.Owner.PhotoName);
                messageResponse.Sender = message.Owner.ToMemberSummary(memberAvatarUrl);
                messageResponse.IsRead = lastReadMessage != null && message.Created <= lastReadMessage.Created;
                messageResponse.ForwardedMessage = message.ForwardedMessage;
            }
            return messageResponse;
        }

        public static ForwardMessage ToForwardMessage(this Message message, Guid forwardMessageId)
        {
            var forwardMessage = new ForwardMessage();
            if (message != null)
            {
                forwardMessage.Id = forwardMessageId;
                forwardMessage.ChannelId = message.ChannelId;
                forwardMessage.Body = message.Body;
                forwardMessage.Created = message.Created;
                forwardMessage.OwnerId = message.OwnerId;
            }
            return forwardMessage;
        }
    }
}