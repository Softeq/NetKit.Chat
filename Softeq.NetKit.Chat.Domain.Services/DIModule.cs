// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using Softeq.NetKit.Chat.Domain.Services.App.Configuration;
using Softeq.NetKit.Chat.Domain.Services.Channel;
using Softeq.NetKit.Chat.Domain.Services.ChannelMember;
using Softeq.NetKit.Chat.Domain.Services.Member;
using Softeq.NetKit.Chat.Domain.Services.Message;

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

            builder.RegisterType<CloudStorageConfiguration>()
                .AsSelf();

            builder.RegisterType<AttachmentConfiguration>()
                .AsSelf();
        }
    }
}