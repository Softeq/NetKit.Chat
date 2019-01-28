// Developed by Softeq Development Corporation
// http://www.softeq.com

using AutoMapper;
using EnsureThat;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.ChannelMember;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.MessageAttachment;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Settings;
using System;
using System.Linq;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage;

namespace Softeq.NetKit.Chat.Domain.Services.Mappings
{
    public class DomainModelsMapper : IDomainModelsMapper
    {
        private readonly IMapper _mapper;

        public DomainModelsMapper(IMapper mapper)
        {
            Ensure.That(mapper).IsNotNull();

            _mapper = mapper;
        }

        public MessageResponse MapToMessageResponse(Message message, DateTimeOffset? lastReadMessageCreated = null)
        {
            var response = new MessageResponse();
            if (message != null)
            {
                response = _mapper.Map(message, response);
                response.IsRead = lastReadMessageCreated != null && message.Created <= lastReadMessageCreated;
            }

            return response;
        }

        public ChannelSummaryResponse MapToChannelSummaryResponse(Channel channel, ChannelMember channelMember, Message lastReadMessage = null)
        {
            var response = new ChannelSummaryResponse();

            if (channelMember != null)
            {
                response = _mapper.Map(channelMember, response);
            }

            if (channel != null)
            {
                response = _mapper.Map(channel, response);
                var lastMessage = channel.Messages.OrderBy(o => o.Created).LastOrDefault();
                if (lastMessage != null)
                {
                    response.LastMessage = MapToMessageResponse(lastMessage, lastReadMessage?.Created);
                }
                response.UnreadMessagesCount = lastReadMessage != null ?
                    channel.Messages.Count(x => x.Created > lastReadMessage.Created) :
                    channel.Messages.Count;
            }

            return response;
        }

        public DirectChannelResponse MapToDirectChannelResponse(Guid directChannelId, Member owner, Member member)
        {
            var firstMember = owner != null ? _mapper.Map<MemberSummary>(owner) : new MemberSummary();
            var secondMember = owner != null ? _mapper.Map<MemberSummary>(member) : new MemberSummary();

            return new DirectChannelResponse
            {
                DirectChannelId = directChannelId,
                Owner = firstMember,
                Member = secondMember
            };
        }

        public AttachmentResponse MapToAttachmentResponse(Attachment attachment)
        {
            return attachment != null ? _mapper.Map<AttachmentResponse>(attachment) : new AttachmentResponse();
        }

        public ChannelResponse MapToChannelResponse(Channel channel)
        {
            return channel != null ? _mapper.Map<ChannelResponse>(channel) : new ChannelResponse();
        }

        public ChannelMemberResponse MapToChannelMemberResponse(ChannelMember channelMember)
        {
            return channelMember != null ? _mapper.Map<ChannelMemberResponse>(channelMember) : new ChannelMemberResponse();
        }

        public ClientResponse MapToClientResponse(Client client)
        {
            return client != null ? _mapper.Map<ClientResponse>(client) : new ClientResponse();
        }

        public MemberSummary MapToMemberSummary(Member member)
        {
            return member != null ? _mapper.Map<MemberSummary>(member) : new MemberSummary();
        }

        public ForwardMessage MapToForwardMessage(Message message)
        {
            return message != null ? _mapper.Map<ForwardMessage>(message) : new ForwardMessage();
        }

        public SettingsResponse MapToSettingsResponse(Settings settings)
        {
            return settings != null ? _mapper.Map<SettingsResponse>(settings) : new SettingsResponse();
        }
    }
}