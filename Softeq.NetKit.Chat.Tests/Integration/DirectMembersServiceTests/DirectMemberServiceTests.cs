// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Autofac;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Xunit;
using CreateDirectMembersRequest = Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectMembers.CreateDirectMembersRequest;

namespace Softeq.NetKit.Chat.Tests.Integration.DirectMembersServiceTests
{
    public class DirectMemberServiceTests : BaseTest
    {
        private const string SaasFirstUserId = "4d048b6c-37b8-499a-a9e3-d3fe5211d5fc";
        private const string SaasSecondUserId = "D7556759-D12D-4E9F-ADC7-A02F409CC74B";

        private readonly Guid _firstMemberId = new Guid("2c47a9d9-faf5-4ac2-92a4-d2770afc58e8");
        private readonly Guid _secondMemberId = new Guid("9A999BF0-AA09-41B4-9305-64031F271B8A");

        private readonly IDirectMemberService _directMemberService;
        private readonly IMemberService _memberService;

        public DirectMemberServiceTests()
        {
            _directMemberService = LifetimeScope.Resolve<IDirectMemberService>();
            _memberService = LifetimeScope.Resolve<IMemberService>();

            var firstMember = new Member
            {
                Id = _firstMemberId,
                LastActivity = DateTimeOffset.UtcNow,
                IsActive = false,
                Status = UserStatus.Online,
                SaasUserId = SaasFirstUserId
            };

            var secondMember = new Member
            {
                Id = _secondMemberId,
                LastActivity = DateTimeOffset.UtcNow,
                IsActive = false,
                Status = UserStatus.Online,
                SaasUserId = SaasSecondUserId
            };

            UnitOfWork.MemberRepository.AddMemberAsync(firstMember).GetAwaiter().GetResult();
            UnitOfWork.MemberRepository.AddMemberAsync(secondMember).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task CreateDirectMemberAsyncTest()
        {
            // Arrange
            var directId = new Guid("1DF71432-00F4-4B3C-82AA-D26BA86F6AF6");

            var request = new CreateDirectMembersRequest(_firstMemberId, _secondMemberId, SaasFirstUserId)
            {
                DirectId = directId
            };

            // Act
            var response = await _directMemberService.CreateDirectMembers(request);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(response.DirectMemberId, directId);
            Assert.Equal(response.FirstDirectMember.Id, _firstMemberId);
            Assert.Equal(response.SecondDirectMember.Id, _secondMemberId);
        }
    }
}
