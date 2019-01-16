// Developed by Softeq Development Corporation
// http://www.softeq.com

using AutoMapper;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.ChannelMember;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.MessageAttachment;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Settings;

namespace Softeq.NetKit.Chat.Domain.Services.Mappings
{
    public class DomainServicesMappingProfile : Profile
    {
        public DomainServicesMappingProfile()
        {
            CreateMap<Settings, SettingsResponse>();

            CreateMap<NotificationSettings, NotificationSettingResponse>();

            CreateMap<Message, ForwardMessage>();

            CreateMap<Message, MessageResponse>()
                .ForMember(d => d.Sender, opt => opt.MapFrom(s => s.Owner))
                // This option depends on LastReadMessage and could be set by MappingHelper class
                .ForMember(d => d.IsRead, opt => opt.Ignore());

            CreateMap<DomainModels.Member, MemberSummary>()
                .ForMember(d => d.AvatarUrl, opt => opt.MapFrom<AvatarUrlValueResolver>())
                .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.Name));

            CreateMap<Channel, ChannelResponse>();

            CreateMap<Channel, ChannelSummaryResponse>()
                .ForMember(d => d.CreatorSaasUserId, opt => opt.MapFrom(s => s.Creator.SaasUserId))
                // These options depend on LastReadMessage and could be set by MappingHelper class
                .ForMember(d => d.LastMessage, opt => opt.Ignore())
                .ForMember(d => d.UnreadMessagesCount, opt => opt.Ignore())
                // These option should be mapped from ChannelMember class. Could be done by MappingHelper class
                .ForMember(d => d.IsMuted, opt => opt.Ignore())
                .ForMember(d => d.IsPinned, opt => opt.Ignore());

            CreateMap<ChannelMember, ChannelSummaryResponse>()
                // These option should be mapped from Channel class. Could be done by MappingHelper class
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Created, opt => opt.Ignore())
                .ForMember(d => d.Updated, opt => opt.Ignore())
                .ForMember(d => d.UnreadMessagesCount, opt => opt.Ignore())
                .ForMember(d => d.Name, opt => opt.Ignore())
                .ForMember(d => d.IsClosed, opt => opt.Ignore())
                .ForMember(d => d.CreatorId, opt => opt.Ignore())
                .ForMember(d => d.Creator, opt => opt.Ignore())
                .ForMember(d => d.CreatorSaasUserId, opt => opt.Ignore())
                .ForMember(d => d.Description, opt => opt.Ignore())
                .ForMember(d => d.WelcomeMessage, opt => opt.Ignore())
                .ForMember(d => d.Type, opt => opt.Ignore())
                .ForMember(d => d.LastMessage, opt => opt.Ignore())
                .ForMember(d => d.PhotoUrl, opt => opt.Ignore());

            CreateMap<Attachment, AttachmentResponse>();

            CreateMap<ChannelMember, ChannelMemberResponse>();

            CreateMap<Client, ClientResponse>()
                .ForMember(d => d.MemberId, opt => opt.MapFrom(s => s.MemberId))
                .ForMember(d => d.ConnectionClientId, opt => opt.MapFrom(s => s.ClientConnectionId))
                .ForMember(d => d.SaasUserId, opt => opt.MapFrom(s => s.Member.SaasUserId))
                .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.Name));
        }
    }
}