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
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Configuration;
using Softeq.NetKit.Chat.Domain.Services.Extensions;
using Softeq.NetKit.Chat.Domain.Services.Mappers;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.ChannelMember;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Settings;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
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
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            Ensure.That(member).WithException(x => new NetKitChatMemberNotFoundException(request.SaasUserId, "Unable to create channel")).IsNotNull();

            var permanentChannelImageUrl = await CopyImageToDestinationContainerAsync(request.PhotoUrl);

            var newChannel = new Channel
            {
                Id = Guid.NewGuid(),
                Created = DateTimeOffset.UtcNow,
                Name = request.Name,
                Description = request.Description,
                WelcomeMessage = request.WelcomeMessage,
                Type = request.Type,
                Members = new List<ChannelMembers>(),
                CreatorId = member.Id,
                Creator = member,
                MembersCount = 0,
                PhotoUrl = permanentChannelImageUrl
            };

            var creator = new ChannelMembers
            {
                ChannelId = newChannel.Id,
                MemberId = member.Id,
                LastReadMessageId = null,
                IsMuted = false
            };

            newChannel.Members.Add(creator);

            if (request.Type == ChannelType.Private && request.AllowedMembers.Any())
            {
                foreach (var saasUserId in request.AllowedMembers)
                {
                    var allowedMember = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
                    Ensure.That(allowedMember).WithException(x => new NetKitChatMemberNotFoundException(request.SaasUserId, "Unable to add member to channel")).IsNotNull();

                    var model = new ChannelMembers
                    {
                        ChannelId = newChannel.Id,
                        MemberId = allowedMember.Id,
                        LastReadMessageId = null
                    };

                    newChannel.Members.Add(model);
                }
            }

            var channelMembers = newChannel.Members.DistinctBy(x => x.MemberId);

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await UnitOfWork.ChannelRepository.AddChannelAsync(newChannel);

                foreach (var channelMember in channelMembers)
                {
                    await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);
                    await UnitOfWork.ChannelRepository.IncrementChannelMembersCount(newChannel.Id);
                }

                transactionScope.Complete();
            }

            var channel = await UnitOfWork.ChannelRepository.GetChannelByIdAsync(newChannel.Id);
            return channel.ToChannelSummaryResponse(creator.IsMuted, null, null, _configuration);
        }

        public async Task<IReadOnlyCollection<ChannelResponse>> GetMemberChannelsAsync(UserRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            Ensure.That(member).WithException(x => new NetKitChatMemberNotFoundException(request.SaasUserId, "Unable to get member channels")).IsNotNull();

            var channels = await UnitOfWork.ChannelRepository.GetChannelsByMemberId(member.Id);

            return channels.Select(x => x.ToChannelResponse(_configuration)).ToList().AsReadOnly();
        }

        public async Task<ChannelResponse> UpdateChannelAsync(UpdateChannelRequest request)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelByIdAsync(request.ChannelId);
            Ensure.That(channel).WithException(x => new NetKitChatChannelNotFoundException(request.ChannelId, "Unable to update channel")).IsNotNull();

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member.Id != channel.CreatorId)
            {
                throw new NetKitChatChannelOwnerRequiredException(request.ChannelId, "Unable to update channel");
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
            Ensure.That(channel).WithException(x => new NetKitChatChannelNotFoundException(request.ChannelId, "Unable to get channel summary")).IsNotNull();

            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);

            var channelMember = await _channelMemberService.GetChannelMemberAsync(new GetChannelMemberRequest(member.Id, request.ChannelId));

            var lastReadMessage = await UnitOfWork.MessageRepository.GetLastReadMessageAsync(member.Id, request.ChannelId);

            return channel.ToChannelSummaryResponse(channelMember.IsMuted, lastReadMessage, member, _configuration);
        }

        public async Task<ChannelResponse> GetChannelByIdAsync(Guid channelId)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelByIdAsync(channelId);
            Ensure.That(channel).WithException(x => new NetKitChatChannelNotFoundException(channelId, "Unable to update channel")).IsNotNull();

            return channel.ToChannelResponse(_configuration);
        }

        public async Task<ChannelResponse> CloseChannelAsync(ChannelRequest request)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelByIdAsync(request.ChannelId);
            Ensure.That(channel).WithException(x => new NetKitChatChannelNotFoundException(request.ChannelId, "Unable to close channel")).IsNotNull();

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            Ensure.That(member).WithException(x => new NetKitChatMemberNotFoundException(request.SaasUserId, "Unable to close channel")).IsNotNull();

            if (member.Id != channel.CreatorId)
            {
                throw new NetKitChatChannelOwnerRequiredException(request.ChannelId, "Unable to close channel");
            }

            channel.IsClosed = true;
            await UnitOfWork.ChannelRepository.UpdateChannelAsync(channel);

            return channel.ToChannelResponse(_configuration);
        }

        public async Task<IReadOnlyCollection<ChannelSummaryResponse>> GetAllowedChannelsAsync(UserRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            Ensure.That(member).WithException(x => new NetKitChatMemberNotFoundException(request.SaasUserId, "Unable to get allowed channels")).IsNotNull();

            var channels = await UnitOfWork.ChannelRepository.GetAllowedChannelsAsync(member.Id);

            var channelsResponse = new List<ChannelSummaryResponse>();
            foreach (var channel in channels)
            {
                var channelMember = await UnitOfWork.ChannelMemberRepository.GetChannelMemberAsync(member.Id, channel.Id);
                var channelCreator = await _memberService.GetMemberByIdAsync(channel.CreatorId.Value);
                if (channelMember.LastReadMessageId != null)
                {
                    var lastReadMessage = await UnitOfWork.MessageRepository.GetMessageByIdAsync((Guid) channelMember.LastReadMessageId);
                    channelsResponse.Add(channel.ToChannelSummaryResponse(channelMember.IsMuted, lastReadMessage, channelCreator, _configuration));
                }
                else
                {
                    channelsResponse.Add(channel.ToChannelSummaryResponse(channelMember.IsMuted, null, channelCreator, _configuration));
                }
            }

            // TODO: Improve performance
            var sortedChannels = channelsResponse
                .Select(x => new
                {
                    Channel = x,
                    SortedDate = x.LastMessage?.Created ?? x.Created
                })
                .OrderByDescending(x => x.SortedDate)
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
            Ensure.That(channel).WithException(x => new NetKitChatChannelNotFoundException(request.ChannelId, "Unable to join to channel")).IsNotNull();

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            Ensure.That(member).WithException(x => new NetKitChatMemberNotFoundException(request.SaasUserId, "Unable to join to channel")).IsNotNull();

            var channelMembers = await _channelMemberService.GetChannelMembersAsync(new ChannelRequest(request.SaasUserId, request.ChannelId));
            if (channelMembers.Any(x => x.MemberId == member.Id))
            {
                throw new NetKitChatMemberAlreadyJoinedException(member.Id, request.ChannelId, "Unable to join to channel");
            }
            
            if (channel.Type == ChannelType.Private && channel.CreatorId != member.Id)
            {
                throw new NetKitChatInsufficientRightsException(member.Id, channel.Id, "Unable to join to channel");
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
            Ensure.That(channel).WithException(x => new NetKitChatChannelNotFoundException(request.ChannelId, "Unable to leave channel")).IsNotNull();

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            Ensure.That(member).WithException(x => new NetKitChatMemberNotFoundException(request.SaasUserId, "Unable to leave channel")).IsNotNull();

            var isMemberExistsInChannel = await UnitOfWork.ChannelRepository.IsMemberExistsInChannelAsync(member.Id, channel.Id);
            if (!isMemberExistsInChannel)
            {
                throw new NetKitChatMemberNotJoinedException(member.Id, channel.Id, "Unable to leave channel");
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
            return await UnitOfWork.ChannelRepository.IsMemberExistsInChannelAsync(request.MemberId, request.ChannelId);
        }

        public async Task MuteChannelAsync(ChannelRequest request)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelByIdAsync(request.ChannelId);
            Ensure.That(channel).WithException(x => new NetKitChatChannelNotFoundException(request.ChannelId, "Unable to mute channel")).IsNotNull();

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            Ensure.That(member).WithException(x => new NetKitChatMemberNotFoundException(request.SaasUserId, "Unable to mute channel")).IsNotNull();

            var isMemberExistsInChannel = await UnitOfWork.ChannelRepository.IsMemberExistsInChannelAsync(member.Id, channel.Id);
            if (!isMemberExistsInChannel)
            {
                throw new NetKitChatMemberNotJoinedException(member.Id, channel.Id, "Unable to leave channel");
            }

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await UnitOfWork.ChannelMemberRepository.MuteChannelAsync(member.Id, channel.Id);

                transactionScope.Complete();
            }
        }

        public async Task<int> GetChannelMessagesCountAsync(Guid channelId)
        {
            return await UnitOfWork.MessageRepository.GetChannelMessagesCountAsync(channelId);
        }
    }
}