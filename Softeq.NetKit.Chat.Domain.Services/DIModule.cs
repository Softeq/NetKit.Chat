// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using Softeq.NetKit.Chat.Domain.Services.Configuration;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;

namespace Softeq.NetKit.Chat.Domain.Services
{
    public class DIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ChannelService>()
                .As<IChannelService>();

            builder.RegisterType<MemberService>()
                .As<IMemberService>();

            builder.RegisterType<MessageService>()
                .As<IMessageService>();

            builder.RegisterType<ChannelMemberService>()
                .As<IChannelMemberService>();

            builder.RegisterType<ClientService>()
                .As<IClientService>();

            builder.RegisterType<CloudStorageConfiguration>()
                .AsSelf();

            builder.RegisterType<AttachmentConfiguration>()
                .AsSelf();
        }
    }
}