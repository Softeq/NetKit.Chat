// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using Softeq.NetKit.Chat.Common.Cache;
using Softeq.NetKit.Chat.Data.Interfaces.SocketConnection;
using Softeq.NetKit.Chat.Domain.Channel;
using Softeq.NetKit.Chat.Domain.ChannelMember;
using Softeq.NetKit.Chat.Domain.Member;
using Softeq.NetKit.Chat.Domain.Message;
using Softeq.NetKit.Chat.Domain.Services.App.Configuration;
using Softeq.NetKit.Chat.Domain.Services.Channel;
using Softeq.NetKit.Chat.Domain.Services.ChannelMember;
using Softeq.NetKit.Chat.Domain.Services.Client;
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

            builder.Register(x =>
            {
                var context = x.Resolve<IComponentContext>();
                var configurationRoot = context.Resolve<IConfiguration>();
                var cfg = new RedisCacheConfiguration();

                
                return cfg;
            });

            builder.RegisterType<DbSocketClientService>()
                .As<IDistributedCacheClient>();

            builder.RegisterType<DbSocketClientService>()
                .As<IClientService>();
            
            builder.Register(x =>
            {
                var context = x.Resolve<IComponentContext>();
                var configurationRoot = context.Resolve<IConfiguration>();

                var cfg = new CloudStorageConfiguration(
                    configurationRoot["AzureStorage:ContentStorageHost"],
                    configurationRoot["AzureStorage:MessageAttachmentsContainer"],
                    configurationRoot["AzureStorage:MemberAvatarsContainer"],
                    configurationRoot["AzureStorage:ChannelImagesContainer"],
                    configurationRoot["AzureStorage:TempContainerName"],
                    Convert.ToInt32(configurationRoot["AzureStorage:MessagePhotoSize"]));
                return cfg;
            });

            builder.Register(x =>
            {
                var context = x.Resolve<IComponentContext>();
                var configurationRoot = context.Resolve<IConfiguration>();

                var cfg = new AttachmentConfiguration(Convert.ToInt32(configurationRoot["MessageAttachments:Limit"]));
                return cfg;
            });
        }
    }
}