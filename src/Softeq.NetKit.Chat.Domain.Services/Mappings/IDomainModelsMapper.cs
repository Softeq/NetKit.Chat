// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Channel;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.ChannelMember;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Member;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Message;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.MessageAttachment;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Settings;
using Softeq.NetKit.Chat.Client.SDK.Models.SignalRModels.Client;
using Softeq.NetKit.Chat.Domain.DomainModels;


namespace Softeq.NetKit.Chat.Domain.Services.Mappings
{
    public interface IDomainModelsMapper
    {
        AttachmentResponse MapToAttachmentResponse(Attachment attachment);
        ChannelMemberResponse MapToChannelMemberResponse(ChannelMember channelMember);
        ChannelResponse MapToChannelResponse(Channel channel);
        ChannelSummaryResponse MapToChannelSummaryResponse(Channel channel, ChannelMember channelMember, Message lastReadMessage = null);
        ChannelSummaryResponse MapToDirectChannelSummaryResponse(Channel channel, DomainModels.Member currentUser,
            DomainModels.Member directMember, Message lastReadMessage = null);
        ClientResponse MapToClientResponse(DomainModels.Client client);
        ForwardMessage MapToForwardMessage(Message message);
        MemberSummaryResponse MapToMemberSummaryResponse(DomainModels.Member member);
        MessageResponse MapToMessageResponse(Message message, DateTimeOffset? lastReadMessageCreated = null);
        SettingsResponse MapToSettingsResponse(Settings settings);
        NotificationSettingResponse MapToNotificationSettingsResponse(NotificationSettings notificationSettings);
    }
}
