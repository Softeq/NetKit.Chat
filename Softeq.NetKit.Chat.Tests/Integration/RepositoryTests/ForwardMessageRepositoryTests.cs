// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.RepositoryTests
{
    public class ForwardMessageRepositoryTests : BaseTest
    {
        private readonly Guid _memberId = new Guid("FE728AF3-DDE7-4B11-BD9B-55C3862262AA");
        private readonly Guid _channelId = new Guid("FE728AF3-DDE7-4B11-BD9B-11C3862262EE");

        public ForwardMessageRepositoryTests()
        {
            var member = new Member
            {
                Id = _memberId,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online,
                Name = "testMember"
            };
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();

            var channel = new Channel
            {
                Id = _channelId,
                CreatorId = member.Id,
                Name = "testMessageChannel",
                Type = ChannelType.Public,
                MembersCount = 0
            };
            UnitOfWork.ChannelRepository.AddChannelAsync(channel).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task AddForwardMessageAsync_ShouldCreateForwardMessage()
        {
            var forwardMessage = new ForwardMessage
            {
                Id = Guid.NewGuid(),
                Body = "Body",
                ChannelId = _channelId,
                OwnerId = _memberId,
                Created = DateTimeOffset.UtcNow
            };

            await UnitOfWork.ForwardMessageRepository.AddForwardMessageAsync(forwardMessage);

            var createdForwardMessage = await UnitOfWork.ForwardMessageRepository.GetForwardMessageAsync(forwardMessage.Id);
            createdForwardMessage.Should().BeEquivalentTo(createdForwardMessage, options => options.Excluding(message => message.Owner));
        }

        [Fact]
        public async Task DeleteForwardMessageAsync_ShouldDeleteForwardMessage()
        {
            var forwardMessage = new ForwardMessage
            {
                Id = Guid.NewGuid(),
                Body = "Body",
                ChannelId = _channelId,
                OwnerId = _memberId,
                Created = DateTimeOffset.UtcNow
            };
            await UnitOfWork.ForwardMessageRepository.AddForwardMessageAsync(forwardMessage);

            await UnitOfWork.ForwardMessageRepository.DeleteForwardMessageAsync(forwardMessage.Id);

            var nullForwardMessage = await UnitOfWork.ForwardMessageRepository.GetForwardMessageAsync(forwardMessage.Id);
            nullForwardMessage.Should().BeNull();
        }

        [Fact]
        public async Task GetForwardMessageByIdAsync_ShouldReturnForwardMessage()
        {
            var forwardMessage = new ForwardMessage
            {
                Id = Guid.NewGuid(),
                Body = "Body",
                ChannelId = _channelId,
                OwnerId = _memberId,
                Created = DateTimeOffset.UtcNow
            };
            await UnitOfWork.ForwardMessageRepository.AddForwardMessageAsync(forwardMessage);

            var foundMessage = await UnitOfWork.ForwardMessageRepository.GetForwardMessageAsync(forwardMessage.Id);

            foundMessage.Should().BeEquivalentTo(forwardMessage);
        }
    }
}
