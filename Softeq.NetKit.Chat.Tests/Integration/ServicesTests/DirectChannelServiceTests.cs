// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectChannel;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.ServicesTests
{
    public class DirectChannelServiceTests : BaseTest
    {
        private const string SaasFirstUserId = "4d048b6c-37b8-499a-a9e3-d3fe5211d5fc";
        private const string SaasSecondUserId = "D7556759-D12D-4E9F-ADC7-A02F409CC74B";

        private readonly Guid _firstMemberId = new Guid("2c47a9d9-faf5-4ac2-92a4-d2770afc58e8");
        private readonly Guid _secondMemberId = new Guid("9A999BF0-AA09-41B4-9305-64031F271B8A");

        private readonly IDirectChannelService _directChannelService;
        private readonly IMemberService _memberService;

        public DirectChannelServiceTests()
        {
            _memberService = LifetimeScope.Resolve<IMemberService>();
            _directChannelService = LifetimeScope.Resolve<IDirectChannelService>();

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
        public async Task CreateDirectChannelAsyncTest()
        {
            // Arrange
            var directId = new Guid("1DF71432-00F4-4B3C-82AA-D26BA86F6AF6");

            var request = new CreateDirectChannelRequest(SaasFirstUserId, _firstMemberId, _secondMemberId)
            {
                DirectChannelId = directId
            };

            // Act
            var response = await _directChannelService.CreateDirectChannelAsync(request);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(response.DirectChannelId, directId);
            Assert.Equal(response.Owner.Id, _firstMemberId);
            Assert.Equal(response.Member.Id, _secondMemberId);
        }

        [Fact]
        public async Task GetDirectChannelByIdAsyncTest()
        {
            // Arrange
            var directId = new Guid("1DF71432-00F4-4B3C-82AA-D26BA86F6AF6");

            var request = new CreateDirectChannelRequest(SaasFirstUserId, _firstMemberId, _secondMemberId) { DirectChannelId = directId };

            await _directChannelService.CreateDirectChannelAsync(request);

            // Act
            var response = await _directChannelService.GetDirectChannelByIdAsync(request.DirectChannelId);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(response.DirectChannelId, directId);
        }

        [Fact]
        public async Task DeleteDirectMessageByIdAsyncTest()
        {
            // Arrange
            var directChannelId = new Guid("9EDBC8A4-2EEC-4FB2-8410-A2852EB8989A");
            var ownerId = new Guid("2C1CFFE1-3656-4100-9364-6D100D006FA0");
            var memberId = new Guid("B2C9F384-B0E1-45BE-B729-4DB1BB44FDBF");

            var owner = new Member
            {
                Id = ownerId,
                Email = "test",
                Role = UserRole.Admin,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test",
                SaasUserId = "test",
                Status = UserStatus.Offline
            };

            var member = new Member
            {
                Id = memberId,
                Email = "test",
                Role = UserRole.User,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test",
                SaasUserId = "test",
                Status = UserStatus.Offline
            };

            UnitOfWork.MemberRepository.AddMemberAsync(owner).GetAwaiter().GetResult();
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();

            UnitOfWork.DirectChannelRepository.CreateDirectChannel(directChannelId, ownerId, memberId).GetAwaiter().GetResult();

            var directMessageRequest = new CreateDirectMessageRequest(owner.SaasUserId, directChannelId, MessageType.Default, "TestBody");

            var directMessageResponse = await _directChannelService.AddMessageAsync(directMessageRequest);
  
            var messageResponse = await _directChannelService.GetMessagesByIdAsync(directMessageResponse.Id);
            Assert.Equal(directMessageResponse.Id, messageResponse.Id);

            // Act
            await _directChannelService.ArchiveMessageAsync(directMessageResponse.Id, owner.SaasUserId);

            // Assert
            NetKitChatNotFoundException ex = Assert.Throws<NetKitChatNotFoundException>(() => _directChannelService.GetMessagesByIdAsync(directMessageResponse.Id).GetAwaiter().GetResult());
            Assert.Equal(ex.Message, $"Unable to get direct message. Message with messageId:{directMessageResponse.Id} is not found.");
        }

        [Fact]
        public async Task UpdateDirectMessageByIdAsyncTest()
        {
            // Arrange
            var directChannelId = new Guid("9EDBC8A4-2EEC-4FB2-8410-A2852EB8989A");
            var ownerId = new Guid("2C1CFFE1-3656-4100-9364-6D100D006FA0");
            var memberId = new Guid("B2C9F384-B0E1-45BE-B729-4DB1BB44FDBF");

            var owner = new Member
            {
                Id = ownerId,
                Email = "test",
                Role = UserRole.Admin,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test",
                SaasUserId = "test",
                Status = UserStatus.Offline
            };

            var member = new Member
            {
                Id = memberId,
                Email = "test",
                Role = UserRole.User,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test",
                SaasUserId = "test",
                Status = UserStatus.Offline
            };

            UnitOfWork.MemberRepository.AddMemberAsync(owner).GetAwaiter().GetResult();
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();

            UnitOfWork.DirectChannelRepository.CreateDirectChannel(directChannelId, ownerId, memberId).GetAwaiter().GetResult();

            var directMessageRequest = new CreateDirectMessageRequest(owner.SaasUserId, directChannelId, MessageType.Default, "TestBody");

            var directMessageResponse = await _directChannelService.AddMessageAsync(directMessageRequest);

            var newDirectMessageRequest = new UpdateDirectMessageRequest(owner.SaasUserId, directMessageResponse.Id, directMessageResponse.DirectChannelId, "NewTestBody");

            // Act
            var result = await _directChannelService.UpdateMessageAsync(newDirectMessageRequest);

            // Assert
            Assert.Equal(result.Id, newDirectMessageRequest.MessageId);
            Assert.Equal(result.Body, newDirectMessageRequest.Body);
        }

        [Fact]
        public async Task GetMessagesByChannelIdAsyncTest()
        {
            var directChannelId = new Guid("9EDBC8A4-2EEC-4FB2-8410-A2852EB8989A");
            var ownerId = new Guid("2C1CFFE1-3656-4100-9364-6D100D006FA0");
            var memberId = new Guid("B2C9F384-B0E1-45BE-B729-4DB1BB44FDBF");

            var owner = new Member
            {
                Id = ownerId,
                Email = "test",
                Role = UserRole.Admin,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test",
                SaasUserId = "test",
                Status = UserStatus.Offline
            };

            var member = new Member
            {
                Id = memberId,
                Email = "test",
                Role = UserRole.User,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test",
                SaasUserId = "test",
                Status = UserStatus.Offline
            };

            UnitOfWork.MemberRepository.AddMemberAsync(owner).GetAwaiter().GetResult();
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();
            UnitOfWork.DirectChannelRepository.CreateDirectChannel(directChannelId, ownerId, memberId).GetAwaiter().GetResult();

            var directMessage01 = new Message
            {
                Body = "TestBody01",
                Created = DateTimeOffset.UtcNow,
                DirectChannelId = directChannelId,
                Id = new Guid("661F903F-246F-434A-AD7A-A5ED76C5919A"),
                OwnerId = ownerId,
                Updated = DateTimeOffset.UtcNow
            };

            var directMessage02 = new Message
            {
                Body = "TestBody02",
                Created = DateTimeOffset.UtcNow,
                DirectChannelId = directChannelId,
                Id = new Guid("6F01ABA0-6DD4-49BC-A6DD-83350E7F6D74"),
                OwnerId = ownerId,
                Updated = DateTimeOffset.UtcNow
            };

            var directMessage03 = new Message
            {
                Body = "TestBody03",
                Created = DateTimeOffset.UtcNow,
                DirectChannelId = directChannelId,
                Id = new Guid("7AF65CEC-788B-4040-9590-96793615D9FD"),
                OwnerId = memberId,
                Updated = DateTimeOffset.UtcNow
            };

            var directMessage04 = new Message
            {
                Body = "TestBody04",
                Created = DateTimeOffset.UtcNow,
                DirectChannelId = directChannelId,
                Id = new Guid("094630E1-95FC-4B9A-ABF9-D89361C44C07"),
                OwnerId = ownerId,
                Updated = DateTimeOffset.UtcNow
            };

            UnitOfWork.MessageRepository.AddMessageAsync(directMessage01).GetAwaiter().GetResult();
            UnitOfWork.MessageRepository.AddMessageAsync(directMessage02).GetAwaiter().GetResult();
            UnitOfWork.MessageRepository.AddMessageAsync(directMessage03).GetAwaiter().GetResult();
            UnitOfWork.MessageRepository.AddMessageAsync(directMessage04).GetAwaiter().GetResult();

            // Act
            var directMessagesResponse = await _directChannelService.GetMessagesByChannelIdAsync(directChannelId);

            // Assert
            Assert.Equal(directMessagesResponse.Count, 4);
            Assert.Equal(directMessagesResponse.Where(x => x.Owner.Id == ownerId).Count(), 3);
            Assert.Equal(directMessagesResponse.Where(x => x.Owner.Id == memberId).Count(), 1);
        }
    }
}
