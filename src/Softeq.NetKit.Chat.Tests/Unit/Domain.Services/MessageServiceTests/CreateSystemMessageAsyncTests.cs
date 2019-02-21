// Developed by Softeq Development Corporation
// http://www.softeq.com

using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.SystemMessage;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.SystemMessage;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MessageServiceTests
{
    public class CreateSystemMessageAsyncTests : MessageServiceTestBase
    {
        [Fact]
        public async Task ShouldCreateSystemMessage()
        {
            // Arrange
            var channelId = new Guid("FBD8E9C2-2A34-4BF1-A603-61BBA9C103C4");
            var memberId = new Guid("AC8D7EA8-5FC6-4D02-8BEF-B50A12F5ECA2");

            var request = new CreateSystemMessageRequest
            {
                Body = "TestBody",
                ChannelId = channelId,
                MemberId = memberId
            };

            var channel = new Channel { Id = channelId };
            var member = new Member { Id = memberId };

            _dateTimeProviderMock.Setup(x => x.GetUtcNow())
                .Returns(DateTimeOffset.UtcNow)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.Is<Guid>(id => id.Equals(request.ChannelId))))
                .ReturnsAsync(channel)
                .Verifiable();

            _memberRepositoryMock.Setup(x =>x.GetMemberByIdAsync(It.Is<Guid>(id =>id.Equals(request.MemberId))))
                .ReturnsAsync(member)
                .Verifiable();

            Message message = null;
            _messageRepositoryMock.Setup(x => x.AddMessageAsync(It.IsAny<Message>()))
                .Callback<Message>(x => message = x)
                .Returns(Task.CompletedTask)
                .Verifiable();

            var response = new SystemMessageResponse();
            _domainModelsMapperMock.Setup(x => x.MapToSystemMessageResponse(
                    It.Is<Message>(m => m.Equals(message)),
                    It.Is<Member>(mem => mem.Equals(member)),
                    It.Is<Channel>(c => c.Equals(channel))))
                .Returns(response)
                .Verifiable();

            // Act
            var act = await _messageService.CreateSystemMessageAsync(request);

            // Assert
            VerifyMocks();

            var eq = act.Equals(response);
            eq.Equals(true);
        }
    }
}
