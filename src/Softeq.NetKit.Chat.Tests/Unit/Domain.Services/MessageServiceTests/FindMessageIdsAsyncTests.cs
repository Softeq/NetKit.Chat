// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MessageServiceTests
{
    public class FindMessageIdsAsyncTests : MessageServiceTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldReturnMessages_WhenLastReadMessageNotFound()
        {
            // Arrange
            var channelId = new Guid("864EB62D-D833-47FA-8A88-DDBFE76AE6A7");
            const string searchText = "some message text";

            var messageIds = new List<Guid>
            {
                new Guid("AE9B09ED-9F67-48A7-BF97-31253A2BD7C8"),
                new Guid("934D2A5F-1824-4FEB-B6DB-1D8BD9599997")
            };

            _messageRepositoryMock.Setup(x => x.FindMessageIdsAsync(
                    It.Is<Guid>(channel => channel.Equals(channelId)),
                    It.Is<string>(text => text.Equals(searchText))))
                .ReturnsAsync(messageIds)
                .Verifiable();

            // Act
            var result = await _messageService.FindMessageIdsAsync(channelId, searchText);

            // Assert
            VerifyMocks();
            result.Should().BeEquivalentTo(messageIds);
        }
    }
}