﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.RepositoryTests
{
    public class MemberRepositoryTests : BaseTest
    {
        private readonly Guid _memberId = new Guid("1a0781d2-f3d8-418a-810c-33e78b457678");
        private Member Member;
        private readonly Guid _channelId = new Guid("0af706fa-d820-4cde-9ccd-c189ed2561da");
        private readonly int _initialTestMembersCount;

        public MemberRepositoryTests()
        {
            Member = new Member
            {
                Id = _memberId,
                Email = "test",
                Role = UserRole.Admin,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test",
                SaasUserId = "test",
                Status = UserStatus.Offline
            };
            UnitOfWork.MemberRepository.AddMemberAsync(Member).GetAwaiter().GetResult();

            _initialTestMembersCount = 1;

            var channel = new Channel
            {
                Id = _channelId,
                CreatorId = Member.Id
            };
            UnitOfWork.ChannelRepository.AddChannelAsync(channel).GetAwaiter().GetResult();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetPagedMembersAsync_ShouldReturnMembersByPages()
        {
            const int membersCount = 21;
            for (var i = 0; i < membersCount; i++)
            {
                var member = new Member
                {
                    Id = Guid.NewGuid(),
                    Role = UserRole.User,
                    IsBanned = true,
                    LastActivity = DateTimeOffset.UtcNow,
                    Status = UserStatus.Online,
                    Name = i % 2 == 0 ? $"{i}EVEN{i}" : $"{i}ODD{i}",
                    SaasUserId = Guid.NewGuid().ToString()
                };
                await UnitOfWork.MemberRepository.AddMemberAsync(member);
            }

            const int pageSize = 10;

            var evenMembersFirstPage = await UnitOfWork.MemberRepository.GetPagedMembersExceptCurrentAsync(1, pageSize, "even", "saasId");
            var evenMembersSecondPage = await UnitOfWork.MemberRepository.GetPagedMembersExceptCurrentAsync(2, pageSize, "even", "saasId");
            var oddMembersFirstPage = await UnitOfWork.MemberRepository.GetPagedMembersExceptCurrentAsync(1, pageSize, "odd", "saasId");
            var oddMembersSecondPage = await UnitOfWork.MemberRepository.GetPagedMembersExceptCurrentAsync(2, pageSize, "odd", "saasId");
            var allMembers = await UnitOfWork.MemberRepository.GetPagedMembersExceptCurrentAsync(1, 100, null, "saasId");

            evenMembersFirstPage.Results.Count().Should().Be(pageSize);
            evenMembersSecondPage.Results.Count().Should().Be(1);
            oddMembersFirstPage.Results.Count().Should().Be(pageSize);
            oddMembersSecondPage.Results.Count().Should().Be(0);
            allMembers.Results.Count().Should().Be(membersCount + _initialTestMembersCount);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetPotentialChannelMembersAsync_ShouldReturnMembersByPages()
        {
            var secondChannel = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = true,
                CreatorId = _memberId,
                Created = DateTimeOffset.UtcNow,
                Type = ChannelType.Public,
                MembersCount = 1
            };
            await UnitOfWork.ChannelRepository.AddChannelAsync(secondChannel);


            const int membersCount = 22;
            for (var i = 0; i < membersCount; i++)
            {
                var isEvenCounter = i % 2 == 0;

                var member = new Member
                {
                    Id = Guid.NewGuid(),
                    Role = UserRole.User,
                    IsBanned = true,
                    LastActivity = DateTimeOffset.UtcNow,
                    Status = UserStatus.Online,
                    Name = isEvenCounter ? $"{i}EVEN{i}" : $"{i}ODD{i}"
                };
                await UnitOfWork.MemberRepository.AddMemberAsync(member);

                var channelMember = new ChannelMember
                {
                    MemberId = member.Id,
                    // Odd members will be potential members for the channel '_channelId'
                    // Even members will be potential members for the second channel
                    ChannelId = isEvenCounter ? _channelId : secondChannel.Id
                };
                await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);
            }

            const int pageSize = 10;

            var potentialEvenMembersForFirstChannel = await UnitOfWork.MemberRepository.GetPotentialChannelMembersAsync(_channelId, 1, pageSize, "even");
            var potentialOddMembersForFirstChannelFirstPage = await UnitOfWork.MemberRepository.GetPotentialChannelMembersAsync(_channelId, 1, pageSize, "odd");
            var potentialOddMembersForFirstChannelSecondPage = await UnitOfWork.MemberRepository.GetPotentialChannelMembersAsync(_channelId, 2, pageSize, "odd");
            var allPotentialMembersForFirstChannel = await UnitOfWork.MemberRepository.GetPotentialChannelMembersAsync(_channelId, 1, 100, null);
            var allPotentialMembersForSecondChannel = await UnitOfWork.MemberRepository.GetPotentialChannelMembersAsync(secondChannel.Id, 1, 100, null);

            potentialEvenMembersForFirstChannel.Results.Count().Should().Be(0);
            potentialOddMembersForFirstChannelFirstPage.Results.Count().Should().Be(pageSize);
            potentialOddMembersForFirstChannelSecondPage.Results.Count().Should().Be(1);
            allPotentialMembersForFirstChannel.Results.Count().Should().Be(membersCount / 2 + _initialTestMembersCount);
            allPotentialMembersForSecondChannel.Results.Count().Should().Be(membersCount / 2 + _initialTestMembersCount);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetMemberByIdAsync_ShouldAddMemberAndReturnMemberById()
        {
            var member = new Member
            {
                Id = Guid.NewGuid(),
                Role = UserRole.User,
                IsBanned = true,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online,
                Email = "Email",
                LastNudged = DateTimeOffset.UtcNow,
                Name = "Name",
                PhotoName = "PhotoName",
                SaasUserId = "SaasUserId"
            };

            await UnitOfWork.MemberRepository.AddMemberAsync(member);

            var newMember = await UnitOfWork.MemberRepository.GetMemberByIdAsync(member.Id);

            newMember.Should().BeEquivalentTo(member);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task ActivateMemberAsync_ShouldActivateMember()
        {
            var member = new Member
            {
                Id = Guid.NewGuid(),
                Role = UserRole.User,
                IsBanned = true,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online,
                IsActive = false,
                Email = "Email",
                LastNudged = DateTimeOffset.UtcNow,
                Name = "Name",
                PhotoName = "PhotoName",
                SaasUserId = "SaasUserId"
            };
            await UnitOfWork.MemberRepository.AddMemberAsync(member);

            var memberToActivate = new Member
            {
                Id = member.Id,
                Role = UserRole.User,
                IsBanned = true,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online,
                IsActive = true,
                Email = "Email",
                LastNudged = DateTimeOffset.UtcNow,
                Name = "Name",
                PhotoName = "PhotoName",
                SaasUserId = "SaasUserId"
            };

            await UnitOfWork.MemberRepository.ActivateMemberAsync(memberToActivate);

            var activatedMember = await UnitOfWork.MemberRepository.GetMemberByIdAsync(member.Id);

            activatedMember.IsActive.Should().Be(memberToActivate.IsActive);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task UpdateMemberAsync_ShouldUpdateMember()
        {
            var member = new Member
            {
                Id = Guid.NewGuid(),
                Role = UserRole.User,
                IsBanned = true,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online,
                Email = "Email",
                LastNudged = DateTimeOffset.UtcNow,
                Name = "Name",
                PhotoName = "PhotoName",
                SaasUserId = "SaasUserId"
            };
            await UnitOfWork.MemberRepository.AddMemberAsync(member);

            var memberToUpdate = new Member
            {
                Id = member.Id,
                Role = UserRole.Admin,
                IsBanned = false,
                LastActivity = member.LastActivity + TimeSpan.FromHours(1),
                Status = UserStatus.Offline,
                Email = "Email2",
                LastNudged = member.LastNudged + TimeSpan.FromHours(1),
                Name = "Name2",
                PhotoName = "PhotoName2",
                SaasUserId = "SaasUserId2"
            };

            await UnitOfWork.MemberRepository.UpdateMemberAsync(memberToUpdate);

            var updatedMember = await UnitOfWork.MemberRepository.GetMemberByIdAsync(member.Id);

            updatedMember.Should().BeEquivalentTo(memberToUpdate);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetMemberBySaasUserIdAsync_ShouldReturnMemberBySaasUserId()
        {
            var member = new Member
            {
                Id = Guid.NewGuid(),
                Role = UserRole.User,
                IsBanned = true,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online,
                Email = "Email",
                LastNudged = DateTimeOffset.UtcNow,
                Name = "Name",
                PhotoName = "PhotoName",
                SaasUserId = "SaasUserId"
            };

            await UnitOfWork.MemberRepository.AddMemberAsync(member);

            var newMember = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(member.SaasUserId);

            newMember.Should().BeEquivalentTo(member);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetAllMembersByChannelIdAsync_ShouldReturnAllMembersInChannel()
        {
            // Arrange

            // Add list of members to expected channel
            var expectedMembers = new List<Member>();
            for (var i = 0; i < 5; i++)
            {
                var member = new Member
                {
                    Id = Guid.NewGuid(),
                    Role = UserRole.User,
                    IsBanned = true,
                    LastActivity = DateTimeOffset.UtcNow,
                    Status = UserStatus.Online,
                    Email = $"Email{i}",
                    LastNudged = DateTimeOffset.UtcNow,
                    Name = $"Name{i}",
                    PhotoName = $"PhotoName{i}",
                    SaasUserId = $"SaasUserId{i}"
                };
                await UnitOfWork.MemberRepository.AddMemberAsync(member);
                expectedMembers.Add(member);

                var channelMember = new ChannelMember
                {
                    MemberId = member.Id,
                    ChannelId = _channelId
                };
                await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);
            }

            // Add second channel and member for this channel
            var secondChannel = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = true,
                CreatorId = _memberId,
                Created = DateTimeOffset.UtcNow,
                Type = ChannelType.Public,
                MembersCount = 1
            };
            await UnitOfWork.ChannelRepository.AddChannelAsync(secondChannel);

            var memberInAnotherChannel = new Member
            {
                Id = Guid.NewGuid(),
                Role = UserRole.User,
                IsBanned = true,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online,
                Email = "Email",
                LastNudged = DateTimeOffset.UtcNow,
                Name = "Name",
                PhotoName = "PhotoName",
                SaasUserId = "SaasUserId"
            };
            await UnitOfWork.MemberRepository.AddMemberAsync(memberInAnotherChannel);

            var anotherChannelMember = new ChannelMember
            {
                MemberId = memberInAnotherChannel.Id,
                ChannelId = secondChannel.Id
            };
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(anotherChannelMember);

            // Add member without channel
            var memberWithoutChannel = new Member
            {
                Id = Guid.NewGuid(),
                Role = UserRole.User,
                IsBanned = true,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online,
                Email = "Email",
                LastNudged = DateTimeOffset.UtcNow,
                Name = "Name",
                PhotoName = "PhotoName",
                SaasUserId = "SaasUserId"
            };
            await UnitOfWork.MemberRepository.AddMemberAsync(memberWithoutChannel);


            // Act
            var membersFromChannel = await UnitOfWork.MemberRepository.GetAllMembersByChannelIdAsync(_channelId);

            // Assert
            membersFromChannel.Should().BeEquivalentTo(expectedMembers);
        }

        [Fact]
        public async Task GetMembersExceptProvidedAsync_ShouldReturnAllMembersExceptProvided()
        {
            // Arrange

            // Add list of members to expected channel
            var expectedMembers = new List<Member>();
            for (var i = 0; i < 5; i++)
            {
                var member = new Member
                {
                    Id = Guid.NewGuid(),
                    Role = UserRole.User,
                    IsBanned = true,
                    LastActivity = DateTimeOffset.UtcNow,
                    Status = UserStatus.Online,
                    Email = $"expectedEmail{i}",
                    LastNudged = DateTimeOffset.UtcNow,
                    Name = $"expectedName{i}",
                    PhotoName = $"expectedPhotoName{i}",
                    SaasUserId = $"expectedSaasUserId{i}"
                };
                await UnitOfWork.MemberRepository.AddMemberAsync(member);
                expectedMembers.Add(member);
            }

            var notExpectedMembers = new List<Member>(){ Member };
            for (var i = 0; i < 5; i++)
            {
                var member = new Member
                {
                    Id = Guid.NewGuid(),
                    Role = UserRole.User,
                    IsBanned = true,
                    LastActivity = DateTimeOffset.UtcNow,
                    Status = UserStatus.Online,
                    Email = $"notExpectedEmail{i}",
                    LastNudged = DateTimeOffset.UtcNow,
                    Name = $"notExpectedName{i}",
                    PhotoName = $"notExpectedPhotoName{i}",
                    SaasUserId = $"notExpectedSaasUserId{i}"
                };
                await UnitOfWork.MemberRepository.AddMemberAsync(member);
                notExpectedMembers.Add(member);
            }

            // Act
            var membersFromChannel = await UnitOfWork.MemberRepository.GetMembersExceptProvidedAsync(notExpectedMembers.Select(x => x.Id));

            // Assert
            membersFromChannel.Should().BeEquivalentTo(expectedMembers);
        }
    }
}
