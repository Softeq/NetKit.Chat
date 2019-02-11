// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using Softeq.NetKit.Chat.Domain;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.ServicesTests
{
    public class ChatSystemMessagesServiceTest : BaseTest
    {
        private readonly IChatSystemMessagesService _chatSystemMessagesService;

        public ChatSystemMessagesServiceTest()
        {
            _chatSystemMessagesService = LifetimeScope.Resolve<IChatSystemMessagesService>();
        }

        [Theory]
        [InlineData(SystemMessagesKey.ChannelLeft, "memberName", "channelName", "memberName left channel channelName")]
        [InlineData(SystemMessagesKey.ChannelJoined, "memberName", "channelName", "memberName joined channel channelName")]
        [InlineData(SystemMessagesKey.ChannelUpdated, "memberName", "channelName", "memberName updated channel channelName")]
        [InlineData(SystemMessagesKey.ChannelClosed, "memberName", "channelName", "memberName closed channel channelName")]
        public void CreateSystemMessageBody(SystemMessagesKey key, string memberName, string channelName, string result)
        {
            var body = _chatSystemMessagesService.FormatSystemMessage(key, memberName, channelName);

            Assert.Equal(body, result);
        }
    }
}
