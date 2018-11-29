// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Tests.Abstract;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.RepositoryTests
{
    public class ForwardMessageRepositoryTests : BaseTest
    {
        private readonly Guid _memberId = new Guid("FE728AF3-DDE7-4B11-BD9B-55C3862262AA");
        private readonly Guid _channelId = new Guid("FE728AF3-DDE7-4B11-BD9B-11C3862262EE");
        private readonly ForwardMessage _forwardMessage;

        public ForwardMessageRepositoryTests()
        {
            var member = new Member
            {
                Id = _memberId,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Active,
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

            _forwardMessage = new ForwardMessage()
            {
                Id = Guid.NewGuid(),
                Body = "test forward message body",
                ChannelId = _channelId,
                OwnerId = _memberId,
                Created = DateTimeOffset.UtcNow
            };
        }

        [Fact]
        public async Task AddForwardMessageAsync_ShouldCreateNewRecord()
        {
            await UnitOfWork.ForwardMessageRepository.AddForwardMessageAsync(_forwardMessage);
            var createdForwardMessage = await UnitOfWork.ForwardMessageRepository.GetForwardMessageByIdAsync(_forwardMessage.Id);

            createdForwardMessage.Should().BeEquivalentTo(createdForwardMessage, options => options.Excluding(message => message.Owner));
        }

        [Fact]
        public async Task DeleteForwardMessageAsync_ShouldDeleteCreatedRecord()
        {
            await UnitOfWork.ForwardMessageRepository.AddForwardMessageAsync(_forwardMessage);
            await UnitOfWork.ForwardMessageRepository.DeleteForwardMessageAsync(_forwardMessage.Id);
            var nullForwardMessage = await UnitOfWork.ForwardMessageRepository.GetForwardMessageByIdAsync(_forwardMessage.Id);

            nullForwardMessage.Should().BeNull("Created message was deleted");
        }

        [Fact]
        public async Task GetForwardMessageByIdAsync_ShouldFindRecordById()
        {
            await UnitOfWork.ForwardMessageRepository.AddForwardMessageAsync(_forwardMessage);
            var findedMessage = await UnitOfWork.ForwardMessageRepository.GetForwardMessageByIdAsync(_forwardMessage.Id);

            findedMessage.Should().BeEquivalentTo(_forwardMessage, options => options
                .Excluding(message => message.Channel)
                .Excluding(message => message.Owner));
        }
    }
}
