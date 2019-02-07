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
        public async Task ShouldCreateSystemDirectMessage()
        {
            // Arrange
            var SaasUserId = "392CA5D5-C427-4D9A-BA21-DFB4EBC1E27A";

            var request = new CreateSystemMessageRequest
            {
                Body = "TestBody",
                ChannelId = new Guid("FBD8E9C2-2A34-4BF1-A603-61BBA9C103C4")
            };

            _dateTimeProviderMock.Setup(x => x.GetUtcNow())
                .Returns(DateTimeOffset.UtcNow)
                .Verifiable();
     
            _channelRepositoryMock.Setup(x => x.IsChannelExistsAsync(It.Is<Guid>(channelId => channelId.Equals(request.ChannelId))))
                .ReturnsAsync(true)
                .Verifiable();

            Message message = null;
            _messageRepositoryMock.Setup(x => x.AddMessageAsync(It.IsAny<Message>()))
                .Callback<Message>(x => message = x)
                .Returns(Task.CompletedTask)
                .Verifiable();

            var response = new SystemMessageResponse();
            _domainModelsMapperMock.Setup(x => x.MapToSystemMessageResponse(It.Is<Message>(m => m.Equals(message))))
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
