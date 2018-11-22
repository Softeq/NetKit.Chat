// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using EnsureThat;
using Softeq.CloudStorage.Extension;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Domain.Channel;
using Softeq.NetKit.Chat.Domain.Channel.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Channel.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.ChannelMember;
using Softeq.NetKit.Chat.Domain.Member;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Services.App.Configuration;
using Softeq.NetKit.Chat.Domain.Services.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Exceptions.ErrorHandling;
using Softeq.NetKit.Chat.Domain.Services.Extensions;
using Softeq.NetKit.Chat.Domain.Services.Settings;
using Softeq.NetKit.Chat.Domain.Settings.TransportModels.Response;

namespace Softeq.NetKit.Chat.Domain.Services.Channel
{
    internal class ChannelService : BaseService, IChannelService
    {
        private readonly IChannelMemberService _channelMemberService;
        private readonly IMemberService _memberService;
        private readonly CloudStorageConfiguration _configuration;
        private readonly IContentStorage _contentStorage;

        public ChannelService(
            IUnitOfWork unitOfWork,
            IChannelMemberService channelMemberService,
            IMemberService memberService,
            CloudStorageConfiguration configuration,
            IContentStorage contentStorage) : base(unitOfWork)
        {
            _channelMemberService = channelMemberService;
            _memberService = memberService;
            _configuration = configuration;
            _contentStorage = contentStorage;
        }

        public async Task<ChannelSummaryResponse> CreateChannelAsync(CreateChannelRequest request)
        {
            var profile = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            Ensure.That(profile)
                .WithException(x => new NotFoundException(new ErrorDto(ErrorCode.NotFound, "Member does not exist.")))
                .IsNotNull();

            string permanentChannelImageUrl = await CopyImageToDestinationContainerAsync(request.PhotoUrl);

            var newChannel = new Domain.Channel.Channel
            {
                Id = Guid.NewGuid(),
                Created = DateTimeOffset.UtcNow,
                Name = request.Name,
                Description = request.Description,
                WelcomeMessage = request.WelcomeMessage,
                Type = request.Type,
                Members = new List<ChannelMembers>(),
                CreatorId = profile.Id,
                Creator = profile,
                MembersCount = 0,
                PhotoUrl = permanentChannelImageUrl
            };

            var creator = new ChannelMembers
            {
                ChannelId = newChannel.Id,
                MemberId = profile.Id,
                LastReadMessageId = null,
                IsMuted = false
            };

            newChannel.Members.Add(creator);

            if (request.Type == ChannelType.Private && request.AllowedMembers.Any())
            {
                foreach (var saasUserId in request.AllowedMembers)
                {
                    var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
                    Ensure.That(member).WithException(x => new NotFoundException(new ErrorDto(ErrorCode.NotFound, "Member does not exist."))).IsNotNull();
                    var model = new ChannelMembers
                    {
                        ChannelId = newChannel.Id,
                        MemberId = member.Id,
                        LastReadMessageId = null
                    };

                    newChannel.Members.Add(model);
                }
            }

            var channelMembers = newChannel.Members.DistinctBy(x => x.MemberId);

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await UnitOfWork.ChannelRepository.AddChannelAsync(newChannel);

                foreach (var member in channelMembers)
                {
                    await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(member);
                    await UnitOfWork.ChannelRepository.IncrementChannelMembersCount(newChannel.Id);
                }

                transactionScope.Complete();
            }

            var channel = await UnitOfWork.ChannelRepository.GetChannelByIdAsync(newChannel.Id);
            return channel.ToChannelSummaryResponse(creator, null, null, _configuration);
        }

        public async Task<IReadOnlyCollection<ChannelResponse>> GetUserChannelsAsync(UserRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            Ensure.That(member)
                .WithException(x => new NotFoundException(new ErrorDto(ErrorCode.NotFound, "Member does not exist.")))
                .IsNotNull();
            var channels = await UnitOfWork.ChannelRepository.GetChannelsByMemberId(member.Id);

            return channels.Select(x => x.ToChannelResponse(_configuration)).ToList().AsReadOnly();
        }

        public async Task<ChannelResponse> UpdateChannelAsync(UpdateChannelRequest request)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelByIdAsync(request.ChannelId);
            Ensure.That(channel)
                .WithException(x => new NotFoundException(new ErrorDto(ErrorCode.NotFound, "Channel does not exist.")))
                .IsNotNull();

            var profile = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (channel.CreatorId != profile.Id)
            {
                throw new AccessForbiddenException(new ErrorDto(ErrorCode.ForbiddenError, "Access forbidden."));
            }

            var permanentChannelImageUrl = await CopyImageToDestinationContainerAsync(request.PhotoUrl);

            channel.Description = request.Topic;
            channel.WelcomeMessage = request.WelcomeMessage;
            channel.Name = request.Name;
            channel.PhotoUrl = permanentChannelImageUrl;
            channel.Updated = DateTimeOffset.UtcNow;

            await UnitOfWork.ChannelRepository.UpdateChannelAsync(channel);
            return channel.ToChannelResponse(_configuration);
        }

        private async Task<string> CopyImageToDestinationContainerAsync(string photoUrl)
        {
            string permanentChannelImageUrl = null;

            if (!string.IsNullOrEmpty(photoUrl))
            {
                var fileName = photoUrl.Substring(photoUrl.LastIndexOf("/", StringComparison.Ordinal) + 1);

                permanentChannelImageUrl = await _contentStorage.CopyBlobAsync(fileName, _configuration.TempContainerName, _configuration.ChannelImagesContainer);
            }

            return permanentChannelImageUrl;
        }

        public async Task<ChannelSummaryResponse> GetChannelSummaryAsync(ChannelRequest request)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelByIdAsync(request.ChannelId);
            Ensure.That(channel)
                .WithException(x => new NotFoundException(new ErrorDto(ErrorCode.NotFound, "Channel does not exist.")))
                .IsNotNull();

            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);
            var channelMember = await UnitOfWork.ChannelMemberRepository.GetChannelMemberAsync(member.Id, request.ChannelId);
            var lastReadMessage = await UnitOfWork.MessageRepository.GetLastReadMessageAsync(member.Id, request.ChannelId);

            return channel.ToChannelSummaryResponse(channelMember, lastReadMessage, member, _configuration);
        }

        public async Task<ChannelResponse> GetChannelByIdAsync(Guid channelId)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelByIdAsync(channelId);
            Ensure.That(channel)
                .WithException(x => new NotFoundException(new ErrorDto(ErrorCode.NotFound, "Channel does not exist.")))
                .IsNotNull();

            return channel.ToChannelResponse(_configuration);
        }

        public async Task<ChannelResponse> CloseChannelAsync(ChannelRequest request)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelByIdAsync(request.ChannelId);
            Ensure.That(channel)
                .WithException(x => new NotFoundException(new ErrorDto(ErrorCode.NotFound, "Channel does not exist.")))
                .IsNotNull();
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            Ensure.That(member)
                .WithException(x => new ServiceException(new ErrorDto(ErrorCode.NotFound, "Member does not exist.")))
                .IsNotNull();
            if (channel.CreatorId != member.Id)
            {
                throw new AccessForbiddenException(new ErrorDto(ErrorCode.ForbiddenError, "Access forbidden."));
            }
            channel.IsClosed = true;
            await UnitOfWork.ChannelRepository.UpdateChannelAsync(channel);

            return channel.ToChannelResponse(_configuration);
        }

        public async Task<IReadOnlyCollection<ChannelSummaryResponse>> GetAllowedChannelsAsync(UserRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            Ensure.That(member)
                .WithException(x => new NotFoundException(new ErrorDto(ErrorCode.NotFound, "Member does not exist.")))
                .IsNotNull();

            var channels = await UnitOfWork.ChannelRepository.GetAllowedChannelsAsync(member.Id);

            var channelsResponse = new List<ChannelSummaryResponse>();
            foreach (var channel in channels)
            {
                var channelMember = await UnitOfWork.ChannelMemberRepository.GetChannelMemberAsync(member.Id, channel.Id);
                var channelCreator = await _memberService.GetMemberByIdAsync(channel.CreatorId.Value);
                if (channelMember.LastReadMessageId != null)
                {
                    var lastReadMessage = await UnitOfWork.MessageRepository.GetMessageByIdAsync((Guid)channelMember.LastReadMessageId);
                    channelsResponse.Add(channel.ToChannelSummaryResponse(channelMember, lastReadMessage, channelCreator, _configuration));
                }
                else
                {
                    channelsResponse.Add(channel.ToChannelSummaryResponse(channelMember, null, channelCreator, _configuration));
                }
            }

            // TODO: Improve performance
            var sortedChannels = channelsResponse.Select(channel => new
            {
                Channel = channel,
                SortedDate = channel.LastMessage?.Created ?? channel.Created
            })
            .OrderByDescending(x => x.Channel.IsPinned)
            .ThenByDescending(x => x.SortedDate)
            .Select(x => x.Channel)
            .ToList()
            .AsReadOnly();

            return sortedChannels;
        }

        public async Task<IReadOnlyCollection<ChannelResponse>> GetAllChannelsAsync()
        {
            var channels = await UnitOfWork.ChannelRepository.GetAllChannelsAsync();
            return channels.Select(x => x.ToChannelResponse(_configuration)).ToList().AsReadOnly();
        }

        public async Task<SettingsResponse> GetChannelSettingsAsync(Guid channelId)
        {
            var settings = await UnitOfWork.SettingRepository.GetSettingsByChannelIdAsync(channelId);
            return settings.ToSettingsResponse();
        }

        public async Task JoinToChannelAsync(JoinToChannelRequest request)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelByIdAsync(request.ChannelId);
            Ensure.That(channel)
                .WithException(x => new NotFoundException(new ErrorDto(ErrorCode.NotFound, "Channel does not exist.")))
                .IsNotNull();
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            Ensure.That(member)
                .WithException(x => new ServiceException(new ErrorDto(ErrorCode.NotFound, "Member does not exist.")))
                .IsNotNull();

            var members = await _channelMemberService.GetChannelMembersAsync(new ChannelRequest(request.SaasUserId, request.ChannelId));
            if (members.Any(x => x.MemberId == member.Id))
            {
                throw new ConflictException(new ErrorDto(ErrorCode.ConflictError, "You have been already joined to the channel."));
            }
            // Throw if the channel is private but the user isn't allowed
            if (channel.Type == ChannelType.Private && channel.CreatorId != member.Id)
            {
                throw new ConflictException(new ErrorDto(ErrorCode.ConflictError, "This channel is not available for you."));
            }

            var channelMember = new ChannelMembers
            {
                ChannelId = channel.Id,
                MemberId = member.Id,
                LastReadMessageId = null
            };

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);
                await UnitOfWork.ChannelRepository.IncrementChannelMembersCount(channel.Id);

                transactionScope.Complete();
            }
        }

        public async Task LeaveChannelAsync(ChannelRequest request)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelByIdAsync(request.ChannelId);
            Ensure.That(channel)
                .WithException(x => new NotFoundException(new ErrorDto(ErrorCode.NotFound, "Channel does not exist.")))
                .IsNotNull();
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            Ensure.That(member)
                .WithException(x => new ServiceException(new ErrorDto(ErrorCode.NotFound, "Member does not exist.")))
                .IsNotNull();
            var ifMemberExist = await UnitOfWork.ChannelRepository.CheckIfMemberExistInChannelAsync(member.Id, channel.Id);
            if (!ifMemberExist)
            {
                throw new ConflictException(new ErrorDto(ErrorCode.ConflictError, "You did not join to this channel."));
            }

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await UnitOfWork.ChannelMemberRepository.DeleteChannelMemberAsync(member.Id, channel.Id);
                await UnitOfWork.ChannelRepository.DecrementChannelMembersCount(channel.Id);

                transactionScope.Complete();
            }
        }

        public async Task<bool> CheckIfMemberExistInChannelAsync(InviteMemberRequest request)
        {
            return await UnitOfWork.ChannelRepository.CheckIfMemberExistInChannelAsync(request.MemberId, request.ChannelId);
        }

        public async Task MuteChannelAsync(ChannelRequest request)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelByIdAsync(request.ChannelId);
            Ensure.That(channel)
                .WithException(x => new NotFoundException(new ErrorDto(ErrorCode.NotFound, "Channel does not exist.")))
                .IsNotNull();
            var member = await GetChannelMemberAsync(request.SaasUserId);
            var ifMemberExist = await UnitOfWork.ChannelRepository.CheckIfMemberExistInChannelAsync(member.Id, request.ChannelId);
            if (!ifMemberExist)
            {
                throw new ConflictException(new ErrorDto(ErrorCode.ConflictError, "You did not join to this channel."));
            }

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await UnitOfWork.ChannelMemberRepository.MuteChannelAsync(member.Id, channel.Id);

                transactionScope.Complete();
            }
        }

        public async Task PinChannelAsync(ChannelRequest request)
        {
            var member = await GetChannelMemberAsync(request.SaasUserId);
            var ifMemberExist = await UnitOfWork.ChannelRepository.CheckIfMemberExistInChannelAsync(member.Id, request.ChannelId);
            if (!ifMemberExist)
            {
                throw new ConflictException(new ErrorDto(ErrorCode.ConflictError, "You did not join to this channel."));
            }
            await UnitOfWork.ChannelMemberRepository.PinChannelAsync(member.Id, request.ChannelId);
        }

        public async Task<int> GetChannelMessagesCountAsync(Guid channelId)
        {
            return await UnitOfWork.MessageRepository.GetChannelMessagesCountAsync(channelId);
        }

        private async Task<Domain.Member.Member> GetChannelMemberAsync(String saasUserId)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            Ensure.That(member)
                .WithException(x => new ServiceException(new ErrorDto(ErrorCode.NotFound, "Member does not exist.")))
                .IsNotNull();

            return member;
        }
    }
}