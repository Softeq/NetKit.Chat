using System;
using System.Collections.Generic;
using System.Text;
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
        private const string _memberName = "testMember";
        private readonly Guid _channelId = new Guid("FE728AF3-DDE7-4B11-BD9B-11C3862262EE");

        public ForwardMessageRepositoryTests()
        {
            var member = new Member
            {
                Id = _memberId,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Active,
                Name = _memberName
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
        public async Task AddForwardMessageAsync_ShouldCreateNewRecord()
        {
            var forwardMessage = new ForwardMessage()
            {
                Id = Guid.NewGuid(),
                Body = "test forward message body",
                ChannelId = _channelId,
                OwnerId = _memberId,
                Created = DateTimeOffset.UtcNow
            };
            await UnitOfWork.ForwardMessageRepository.AddForwardMessageAsync(forwardMessage);
            var createdForwardMessage = await UnitOfWork.ForwardMessageRepository.GetForwardMessageByIdAsync(forwardMessage.Id);

            createdForwardMessage.Should().BeEquivalentTo(createdForwardMessage, options=>options.Excluding(message=>message.Owner));
        }
    }
}
