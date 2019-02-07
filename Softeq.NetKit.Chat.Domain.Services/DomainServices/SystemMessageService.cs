// Developed by Softeq Development Corporation
// http://www.softeq.com

using EnsureThat;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Domain.Services.Utility;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.SystemMessage;
using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    internal class SystemMessageService : BaseService, ISystemMessageService
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public SystemMessageService(IUnitOfWork unitOfWork, IDomainModelsMapper domainModelsMapper, IDateTimeProvider dateTimeProvider) : base(unitOfWork, domainModelsMapper)
        {
            Ensure.That(dateTimeProvider).IsNotNull();

            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<SystemMessageResponse> CreateMessageAsync(Guid channelId, string body)
        {
            // TODO
            var channel = await UnitOfWork.DirectChannelRepository.GetDirectChannelAsync(channelId);
            if (channel == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get direct channel. Chat with {nameof(channelId)}:{channelId} is not found.");
            }

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChannelId = channelId,
                Body = body,
                Created = _dateTimeProvider.GetUtcNow(),
                Type = MessageType.Notification
            }
        }
    }
}
