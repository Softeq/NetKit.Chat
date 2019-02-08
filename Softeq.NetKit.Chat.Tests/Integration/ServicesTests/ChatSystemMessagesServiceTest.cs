// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using Softeq.NetKit.Chat.Application.Services.Services.SystemMessages;
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
        [InlineData("ChannelLeft", "memberName", "channelName", "memberName left channel channelName")]
        [InlineData("ChannelJoined", "memberName", "channelName", "memberName joined channel channelName")]
        [InlineData("ChannelUpdated", "memberName", "channelName", "memberName updated channel channelName")]
        [InlineData("ChannelClosed", "memberName", "channelName", "memberName closed channel channelName")]
        public void CreateSystemMessageBody(string key, string memberName, string channelName, string result)
        {
            var body = _chatSystemMessagesService.FormatSystemMessage(key, memberName, channelName);

            Assert.Equal(body, result);
        }
    }
}
