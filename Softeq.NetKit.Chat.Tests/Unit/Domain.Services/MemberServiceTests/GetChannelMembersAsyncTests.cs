// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberServiceTests
{
    public class GetChannelMembersAsyncTests : MemberServiceTestBase
    {
        [Fact]
        public void ShouldThrowIfChannelDoesNotExist()
        {
            // Arrange
            var channelId = new Guid("BE5C68F1-5983-4C08-B57B-FD4EFD7295B8");

            _channelRepositoryMock.Setup(x => x.IsChannelExistsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _memberService.GetChannelMembersAsync(channelId);

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>().And.Message.Should()
                .Be($"Unable to get channel members. Channel {nameof(channelId)}:{channelId} not found.");

            VerifyMocks();
        }

        [Fact]
        public async Task ShouldReturnMemberSummaryCollection()
        {
            // Arrange
            var channelId = new Guid("BE5C68F1-5983-4C08-B57B-FD4EFD7295B8");

            _channelRepositoryMock.Setup(x => x.IsChannelExistsAsync(It.Is<Guid>(c => c.Equals(channelId))))
                .ReturnsAsync(true)
                .Verifiable();

            List<Member> members = new List<Member>();
            _memberRepositoryMock.Setup(x => x.GetAllMembersByChannelIdAsync(It.Is<Guid>(c => c.Equals(channelId))))
                .ReturnsAsync(members)
                .Verifiable();

            List<MemberSummary> memberSummaries = new List<MemberSummary>();
            foreach (var member in members)
            {
                var memberSummary = new MemberSummary();
                _domainModelsMapperMock.Setup(x => x.MapToMemberSummary(It.Is<Member>(m => m.Equals(member))))
                    .Returns(memberSummary)
                    .Verifiable();

                memberSummaries.Add(memberSummary);
            }

            // Act
            var act = await _memberService.GetChannelMembersAsync(channelId);

            // Assert
            VerifyMocks();

            act.Should().AllBeEquivalentTo(memberSummaries);
        }
    }
}
