﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using AutoMapper;
using EnsureThat;
using Softeq.NetKit.Chat.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Softeq.NetKit.Chat.TransportModels.Enums;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Channel;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.ChannelMember;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Member;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Message;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.MessageAttachment;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Settings;
using Softeq.NetKit.Chat.TransportModels.Models.SignalRModels.Client;

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

        public ChannelSummaryResponse MapToChannelSummaryResponse(ChannelMemberAggregate channelMemberAggregate, Channel channel)
        {
            var response = new ChannelSummaryResponse();

            if (channelMemberAggregate.ChannelMember != null)
            {
                response = _mapper.Map(channelMemberAggregate.ChannelMember, response);
            }

            if (channel != null)
            {
                response = _mapper.Map(channel, response);

                var member = MapToMemberSummaryResponse(channelMemberAggregate.ChannelMember.Member);
                member.Role = channelMemberAggregate.ChannelMember.Role;

                response.Members = new List<MemberSummaryResponse>
                {
                    member
                };
                
                if (channelMemberAggregate.Message != null)
                {
                    response.LastMessage = MapToMessageResponse(channelMemberAggregate.Message);
                }

                response.UnreadMessagesCount = channelMemberAggregate.UnreadMessagesCount;
            }

            return response;
        }

        public ChannelSummaryResponse MapToChannelSummaryResponse(Channel channel, ChannelMember channelMember, Message lastReadMessage = null)
        {
            var response = new ChannelSummaryResponse
            {
                Members = new List<MemberSummaryResponse>()
            };
            
            if (channel != null)
            {
                response = _mapper.Map(channel, response);

                if (channel.Creator != null && channelMember != null)
                {
                    response = _mapper.Map(channelMember, response);

                    var creator = MapToMemberSummaryResponse(channel.Creator);
                    creator.Role = channelMember.Role;
                    response.Members.Add(creator);
                }

                if (channel.Messages == null)
                {
                    response.UnreadMessagesCount = 0;
                    return response;
                }

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

        public ChannelSummaryResponse MapToDirectChannelSummaryResponse(ChannelMemberAggregate channelMemberAggregate, Channel channel, Member directMember)
        {
            var response = new ChannelSummaryResponse();

            if (channel != null)
            {
                response = _mapper.Map(channel, response);

                var member = MapToMemberSummaryResponse(channelMemberAggregate.ChannelMember.Member);
                member.Role = channelMemberAggregate.ChannelMember.Role;

                response.Members = new List<MemberSummaryResponse>
                {
                    member
                };

                if (channelMemberAggregate.Message != null)
                {
                    response.LastMessage = MapToMessageResponse(channelMemberAggregate.Message);
                }

                response.UnreadMessagesCount = channelMemberAggregate.UnreadMessagesCount;
            }

            if (directMember != null)
            {
                var direct = MapToMemberSummaryResponse(directMember);
                direct.Role = UserRole.Admin;
                response.Members.Add(direct);
            }

            return response;
        }

        public ChannelSummaryResponse MapToDirectChannelSummaryResponse(Channel channel, Member currentUser, Member directMember, Message lastReadMessage = null)
        {
            var response = new ChannelSummaryResponse();

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

            if (currentUser != null)
            {
                var current = MapToMemberSummaryResponse(currentUser);
                current.Role = UserRole.Admin;
                response.Members.Add(current);
            }

            if (directMember != null)
            {
                var direct = MapToMemberSummaryResponse(directMember);
                direct.Role = UserRole.Admin;
                response.Members.Add(direct);
            }

            return response;
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

        public MemberSummaryResponse MapToMemberSummaryResponse(Member member)
        {
            return member != null ? _mapper.Map<MemberSummaryResponse>(member) : new MemberSummaryResponse();
        }

        public ForwardMessage MapToForwardMessage(Message message)
        {
            return message != null ? _mapper.Map<ForwardMessage>(message) : new ForwardMessage();
        }

        public SettingsResponse MapToSettingsResponse(Settings settings)
        {
            return settings != null ? _mapper.Map<SettingsResponse>(settings) : new SettingsResponse();
        }

        public NotificationSettingResponse MapToNotificationSettingsResponse(NotificationSettings notificationSettings)
        {
            return notificationSettings != null ? _mapper.Map<NotificationSettingResponse>(notificationSettings) : new NotificationSettingResponse();
        }
    }
}
