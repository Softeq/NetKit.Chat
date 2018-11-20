// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Softeq.NetKit.Chat.Domain.Channel;
using Softeq.NetKit.Chat.Domain.Channel.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Member;
using Softeq.NetKit.Chat.Tests.Abstract;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.ServicesTests
{
    public class MemberServiceTests : BaseTest
    {
        private readonly Guid _memberId = new Guid("2c47a9d9-faf5-4ac2-92a4-d2770afc58e8");
        private readonly Guid _channelId = new Guid("d279799f-c205-4378-b734-0aef519d19f9");

        private const string SaasUserId = "4d048b6c-37b8-499a-a9e3-d3fe5211d5fc";

        private readonly IMemberService _memberService;

        public MemberServiceTests()
        {
            _memberService = LifetimeScope.Resolve<IMemberService>();

            var member = new Member
            {
                Id = _memberId,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Active,
                SaasUserId = SaasUserId
            };
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();

            var channel = new Channel
            {
                Id = _channelId,
                CreatorId = member.Id,
                Name = "testMemberService",
                MembersCount = 0,
                Type = ChannelType.Public
            };
            UnitOfWork.ChannelRepository.AddChannelAsync(channel).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task GetChannelMembersAsyncTest()
        {
            // Arrange
            await _memberService.InviteMemberAsync(new InviteMemberRequest(SaasUserId, _channelId, _memberId));

            // Act
            var members = await _memberService.GetChannelMembersAsync(_channelId);

            // Assert
            Assert.NotNull(members);
            Assert.NotEmpty(members);
            Assert.Equal(_memberId, members.First().Id);
            Assert.Equal(SaasUserId, members.First().SaasUserId);
            Assert.Equal(UserStatus.Active, members.First().Status);
        }

        [Fact]
        public async Task InviteMemberAsyncTest()
        {
            // Act
            var members = await _memberService.GetChannelMembersAsync(_channelId);
            var channel = await _memberService.InviteMemberAsync(new InviteMemberRequest(SaasUserId, _channelId, _memberId));


            // Assert
            Assert.NotNull(channel);
            Assert.True(channel.MembersCount > 0);
            Assert.True(members.Count() < channel.MembersCount);
        }
    }
}