// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.ChannelMember;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMembers;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.MessageAttachment;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Settings;

namespace Softeq.NetKit.Chat.Domain.Services.Mappings
{
    public interface IDomainModelsMapper
    {
        AttachmentResponse MapToAttachmentResponse(Attachment attachment);
        ChannelMemberResponse MapToChannelMemberResponse(ChannelMember channelMember);
        ChannelResponse MapToChannelResponse(Channel channel);
        ChannelSummaryResponse MapToChannelSummaryResponse(Channel channel, ChannelMember channelMember, Message lastReadMessage = null);
        ClientResponse MapToClientResponse(Client client);
        ForwardMessage MapToForwardMessage(Message message);
        MemberSummary MapToMemberSummary(Member member);
        MessageResponse MapToMessageResponse(Message message, DateTimeOffset? lastReadMessageCreated = null);
        SettingsResponse MapToSettingsResponse(Settings settings);
        CreateDirectMembersResponse MapToDirectMembersResponse(Guid directMemberId, Member firstDirectMember, Member secondDirectMember);
    }
}