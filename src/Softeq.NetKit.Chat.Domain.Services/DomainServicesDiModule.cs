// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Autofac;
using Softeq.NetKit.Chat.Domain.Services.Configuration;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Domain.Services.Utility;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Softeq.NetKit.Chat.Domain.Services
{
    public class DomainServicesDiModule : Module
    {
        private const string MessageAttachmentsLimit = "Message:MessageAttachmentsLimit";
        private const string LastMessageReadCount = "Message:LastMessageReadCount";
        private const string MemberJoined = "SystemMessagesTemplates:MemberJoined";
        private const string MemberDeleted = "SystemMessagesTemplates:MemberDeleted";
        private const string MemberLeft = "SystemMessagesTemplates:MemberLeft";
        private const string ChannelNameChanged = "SystemMessagesTemplates:ChannelNameChanged";
        private const string ChannelIconChanged = "SystemMessagesTemplates:ChannelIconChanged";

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x =>
                {
                    var context = x.Resolve<IComponentContext>();
                    var config = context.Resolve<IConfiguration>();
                    return new MessagesConfiguration(
                        Convert.ToInt32(config[MessageAttachmentsLimit]),
                        Convert.ToInt32(config[LastMessageReadCount]));
                }).AsSelf();

            builder.Register(x =>
                {
                    var context = x.Resolve<IComponentContext>();
                    var config = context.Resolve<IConfiguration>();

                    return new SystemMessagesConfiguration(
                        config[MemberJoined],
                        config[MemberDeleted],
                        config[MemberLeft],
                        config[ChannelNameChanged],
                        config[ChannelIconChanged]);
                })
                .As<SystemMessagesConfiguration>();

            builder.RegisterType<ChannelService>()
                .As<IChannelService>();

            builder.RegisterType<MemberService>()
                .As<IMemberService>();

            builder.RegisterType<MessageService>()
                .As<IMessageService>();

            builder.RegisterType<NotificationSettingsService>()
                .As<INotificationSettingsService>();

            builder.RegisterType<ChannelMemberService>()
                .As<IChannelMemberService>();

            builder.RegisterType<ClientService>()
                .As<IClientService>();

            builder.RegisterType<SystemDateTimeProvider>()
                .As<IDateTimeProvider>();

            RegisterMappings(builder);
        }

        private void RegisterMappings(ContainerBuilder builder)
        {
            builder.RegisterType<DomainModelsMapper>()
                .As<IDomainModelsMapper>();
        }
    }
}