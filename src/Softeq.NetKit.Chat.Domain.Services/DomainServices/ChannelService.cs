// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using EnsureThat;
using Softeq.NetKit.Chat.Data.Cloud.DataProviders;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Extensions;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Domain.Services.Utility;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Settings;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    internal class ChannelService : BaseService, IChannelService
    {
        private readonly IMemberService _memberService;
        private readonly ICloudImageProvider _cloudImageProvider;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ChannelService(
            IUnitOfWork unitOfWork,
            IDomainModelsMapper domainModelsMapper,
            IMemberService memberService,
            ICloudImageProvider cloudImageProvider,
            IDateTimeProvider dateTimeProvider)
            : base(unitOfWork, domainModelsMapper)
        {
            Ensure.That(memberService).IsNotNull();
            Ensure.That(cloudImageProvider).IsNotNull();
            Ensure.That(dateTimeProvider).IsNotNull();

            _memberService = memberService;
            _cloudImageProvider = cloudImageProvider;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<ChannelSummaryResponse> CreateChannelAsync(CreateChannelRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to create channel. Member with {nameof(request.SaasUserId)}:{request.SaasUserId} is not found.");
            }

            var permanentChannelImageUrl = await _cloudImageProvider.CopyImageToDestinationContainerAsync(request.PhotoUrl);

            var newChannel = new Channel
            {
                Id = Guid.NewGuid(),
                Created = _dateTimeProvider.GetUtcNow(),
                Name = request.Name,
                Description = request.Description,
                WelcomeMessage = request.WelcomeMessage,
                Type = request.Type,
                Members = new List<ChannelMember>(),
                CreatorId = member.Id,
                Creator = member,
                MembersCount = 0,
                PhotoUrl = permanentChannelImageUrl
            };
            //TODO: [ek] Make creator as nullable. Do not set creator for direct chat. Disable close functionality
            var creator = new ChannelMember
            {
                ChannelId = newChannel.Id,
                MemberId = member.Id,
                LastReadMessageId = null,
                IsMuted = false
            };

            newChannel.Members.Add(creator);

            if ((request.Type == ChannelType.Private || request.Type == ChannelType.Direct) && request.AllowedMembers != null && request.AllowedMembers.Any())
            {
                foreach (var memberId in request.AllowedMembers)
                {
                    var allowedMember = await UnitOfWork.MemberRepository.GetMemberByIdAsync(Guid.Parse(memberId));
                    if (allowedMember == null)
                    {
                        throw new NetKitChatNotFoundException($"Unable to add member to channel. Member {nameof(memberId)}:{memberId} not found.");
                    }

                    var model = new ChannelMember
                    {
                        ChannelId = newChannel.Id,
                        MemberId = allowedMember.Id,
                        LastReadMessageId = null
                    };

                    newChannel.Members.Add(model);
                }
            }

            var channelMembers = newChannel.Members.DistinctBy(x => x.MemberId);

            await UnitOfWork.ExecuteTransactionAsync(async () =>
            {
                await UnitOfWork.ChannelRepository.AddChannelAsync(newChannel);

                foreach (var channelMember in channelMembers)
                {
                    await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);
                    await UnitOfWork.ChannelRepository.IncrementChannelMembersCountAsync(newChannel.Id);
                }
            });

            var channel = await UnitOfWork.ChannelRepository.GetChannelWithCreatorAsync(newChannel.Id);

            return DomainModelsMapper.MapToChannelSummaryResponse(channel, creator);
        }

        public async Task<ChannelSummaryResponse> CreateDirectChannelAsync(CreateDirectChannelRequest request)
        {
            var creator = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (creator == null)
            {
                throw new NetKitChatNotFoundException($"Unable to create channel. Member with {nameof(request.SaasUserId)}:{request.SaasUserId} is not found.");
            }
            var member = await UnitOfWork.MemberRepository.GetMemberByIdAsync(request.MemberId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to create channel. Member with {nameof(request.MemberId)}:{request.MemberId} is not found.");
            }

            var exitingChannelId = await UnitOfWork.ChannelRepository.GetDirectChannelForMembersAsync(creator.Id, member.Id);
            if (exitingChannelId != default(Guid))
            {
                // open existing direct channel
                var existingDirectChannel = await UnitOfWork.ChannelRepository.GetChannelAsync(exitingChannelId);

                existingDirectChannel.Messages = (await UnitOfWork.MessageRepository.GetAllChannelMessagesWithOwnersAsync(exitingChannelId)).ToList();

                existingDirectChannel.IsClosed = false;
                await UnitOfWork.ChannelRepository.UpdateChannelAsync(existingDirectChannel);

                var channelSummary = DomainModelsMapper.MapToDirectChannelSummaryResponse(existingDirectChannel, creator, member);

                //TODO need to send channel summary specific for every user
                channelSummary.LastMessage = null;
                return channelSummary;
            }

            var newChannel = new Channel
            {
                Id = Guid.NewGuid(),
                Created = _dateTimeProvider.GetUtcNow(),
                Type = ChannelType.Direct,
                //TODO: [ek] do not use creator for direct channels
                CreatorId = creator.Id,
                Creator = creator,
                Members = new List<ChannelMember>(),
                MembersCount = 2,
            };
            //TODO: [ek] Make creator as nullable. Do not set creator for direct chat. Disable close functionality
            var creatorChannelMember = new ChannelMember
            {
                ChannelId = newChannel.Id,
                MemberId = creator.Id,
                LastReadMessageId = null,
                IsMuted = false
            };
            newChannel.Members.Add(creatorChannelMember);
            newChannel.Members.Add(new ChannelMember
            {
                ChannelId = newChannel.Id,
                MemberId = member.Id,
                LastReadMessageId = null,
                IsMuted = false
            });

            await UnitOfWork.ExecuteTransactionAsync(async () =>
            {
                await UnitOfWork.ChannelRepository.AddChannelAsync(newChannel);

                foreach (var channelMember in newChannel.Members)
                {
                    await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);
                }
            });

            var channel = await UnitOfWork.ChannelRepository.GetChannelWithCreatorAsync(newChannel.Id);

            return DomainModelsMapper.MapToDirectChannelSummaryResponse(channel, creator, member);
        }

        public async Task<IReadOnlyCollection<ChannelResponse>> GetMemberChannelsAsync(string saasUserId)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get member channels. Member {nameof(saasUserId)}:{saasUserId} is not found.");
            }

            var channels = await UnitOfWork.ChannelRepository.GetAllowedChannelsAsync(member.Id);
            return channels.Select(channel => DomainModelsMapper.MapToChannelResponse(channel)).ToList().AsReadOnly();
        }

        public async Task<ChannelResponse> UpdateChannelAsync(UpdateChannelRequest request)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelAsync(request.ChannelId);
            if (channel == null)
            {
                throw new NetKitChatNotFoundException($"Unable to update channel. Channel {nameof(request.ChannelId)}:{request.ChannelId} is not found.");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member.Id != channel.CreatorId)
            {
                throw new NetKitChatAccessForbiddenException($"Unable to update channel {nameof(request.ChannelId)}:{request.ChannelId}. Channel owner required.");
            }
            if (channel.Type == ChannelType.Direct)
            {
                throw new NetKitChatInvalidOperationException($"Unable to update direct channel {nameof(request.ChannelId)}:{request.ChannelId}.");
            }

            var permanentChannelImageUrl = await _cloudImageProvider.CopyImageToDestinationContainerAsync(request.PhotoUrl);

            channel.Description = request.Description;
            channel.WelcomeMessage = request.WelcomeMessage;
            channel.Name = request.Name;
            channel.PhotoUrl = permanentChannelImageUrl;
            channel.Updated = _dateTimeProvider.GetUtcNow();

            await UnitOfWork.ChannelRepository.UpdateChannelAsync(channel);
            return DomainModelsMapper.MapToChannelResponse(channel);
        }

        public async Task<ChannelSummaryResponse> GetChannelSummaryAsync(string saasUserId, Guid channelId)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelWithCreatorAsync(channelId);
            if (channel == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get channel summary. Channel {nameof(channelId)}:{channelId} is not found.");
            }

            var messages = await UnitOfWork.MessageRepository.GetAllChannelMessagesWithOwnersAsync(channel.Id);
            channel.Messages = messages.ToList();

            var member = await _memberService.GetMemberBySaasUserIdAsync(saasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get channel summary. Member {nameof(saasUserId)}:{saasUserId} is not found.");
            }

            var channelMember = await UnitOfWork.ChannelMemberRepository.GetChannelMemberAsync(member.Id, channelId);
            var lastReadMessage = await UnitOfWork.MessageRepository.GetLastReadMessageAsync(member.Id, channelId);

            return DomainModelsMapper.MapToChannelSummaryResponse(channel, channelMember, lastReadMessage);
        }

        public async Task<ChannelResponse> GetChannelByIdAsync(Guid channelId)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelAsync(channelId);
            if (channel == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get channel by {nameof(channelId)}. Channel {nameof(channelId)}:{channelId} is not found.");
            }

            return DomainModelsMapper.MapToChannelResponse(channel);
        }

        public async Task<ChannelResponse> CloseChannelAsync(string saasUserId, Guid channelId)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelAsync(channelId);
            if (channel == null)
            {
                throw new NetKitChatNotFoundException($"Unable to close channel. Channel {nameof(channelId)}:{channelId} is not found.");
            }

            if (channel.IsClosed)
            {
                throw new NetKitChatInvalidOperationException($"Unable to close channel. Channel {nameof(channelId)}:{channelId} already closed.");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to close channel. Member {nameof(saasUserId)}:{saasUserId} is not found.");
            }

            if (member.Id != channel.CreatorId && channel.Type != ChannelType.Direct)
            {
                throw new NetKitChatAccessForbiddenException($"Unable to close channel {nameof(channelId)}:{channelId}. Channel owner required.");
            }

            channel.IsClosed = true;
            await UnitOfWork.ChannelRepository.UpdateChannelAsync(channel);

            return DomainModelsMapper.MapToChannelResponse(channel);
        }

        public async Task<IReadOnlyCollection<ChannelSummaryResponse>> GetAllowedChannelsAsync(string saasUserId)
        {
            var currentUser = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            if (currentUser == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get allowed channels. Member {nameof(saasUserId)}:{saasUserId} is not found.");
            }

            var channelsResponse = new List<ChannelSummaryResponse>();

            var channels = await UnitOfWork.ChannelRepository.GetAllowedChannelsWithMessagesAndCreatorAsync(currentUser.Id);
            foreach (var channel in channels)
            {
                channel.Messages = channel.Messages?.Where(x => x.AccessibilityStatus == AccessibilityStatus.Present).ToList();
                if (channel.Type == ChannelType.Direct)
                {
                    var members = await UnitOfWork.ChannelMemberRepository.GetChannelMembersWithMemberDetailsAsync(channel.Id);
                    var existingUserChannelMember = members.First(x => x.MemberId == currentUser.Id);

                    //TODO: Extend existing query to make one single request to get all data for both direct and group channel                    
                    var directChannelMember = members.First(x => x.MemberId != currentUser.Id);
                    var member = directChannelMember.Member;
                    ChannelSummaryResponse channelSummaryResponse;
                    if (existingUserChannelMember.LastReadMessageId.HasValue)
                    {
                        var lastReadMessage = await UnitOfWork.MessageRepository.GetAsync(existingUserChannelMember.LastReadMessageId.Value);
                        channelSummaryResponse = DomainModelsMapper.MapToDirectChannelSummaryResponse(channel, currentUser, member, lastReadMessage);
                    }
                    else
                    {
                        channelSummaryResponse = DomainModelsMapper.MapToDirectChannelSummaryResponse(channel, currentUser, member);
                    }
                    channelsResponse.Add(channelSummaryResponse);
                }
                else
                {
                    var existingUserChannelMember = await UnitOfWork.ChannelMemberRepository.GetChannelMemberWithMemberDetailsAsync(currentUser.Id, channel.Id);

                    if (existingUserChannelMember.LastReadMessageId.HasValue)
                    {
                        var lastReadMessage = await UnitOfWork.MessageRepository.GetAsync(existingUserChannelMember.LastReadMessageId.Value);
                        channelsResponse.Add(DomainModelsMapper.MapToChannelSummaryResponse(channel, existingUserChannelMember, lastReadMessage));
                    }
                    else
                    {
                        channelsResponse.Add(DomainModelsMapper.MapToChannelSummaryResponse(channel, existingUserChannelMember));
                    }
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
            return channels.Select(channel => DomainModelsMapper.MapToChannelResponse(channel)).ToList().AsReadOnly();
        }

        public async Task<SettingsResponse> GetChannelSettingsAsync(Guid channelId)
        {
            var settings = await UnitOfWork.SettingRepository.GetSettingsByChannelIdAsync(channelId);
            if (settings == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get channel settings. Settings with {nameof(channelId)}:{channelId} is not found.");
            }

            return DomainModelsMapper.MapToSettingsResponse(settings);
        }

        public async Task JoinToChannelAsync(string saasUserId, Guid channelId)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelAsync(channelId);
            if (channel == null)
            {
                throw new NetKitChatNotFoundException($"Unable to join channel. Channel {nameof(channelId)}:{channelId} is not found.");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to join channel. Member {nameof(saasUserId)}:{saasUserId} is not found.");
            }

            if (channel.Type == ChannelType.Private && channel.CreatorId != member.Id)
            {
                throw new NetKitChatAccessForbiddenException("Unable to join private channel.");
            }

            if (channel.Type == ChannelType.Direct)
            {
                throw new NetKitChatInvalidOperationException($"Unable to join direct channel.");
            }

            var isMemberExistsInChannel = await UnitOfWork.ChannelRepository.IsMemberExistsInChannelAsync(member.Id, channel.Id);
            if (isMemberExistsInChannel)
            {
                throw new NetKitChatInvalidOperationException($"Unable to join channel. Member {nameof(member.Id)}:{member.Id} already joined channel {nameof(channelId)}:{channelId}.");
            }

            var channelMember = new ChannelMember
            {
                ChannelId = channel.Id,
                MemberId = member.Id,
                LastReadMessageId = null
            };

            await UnitOfWork.ExecuteTransactionAsync(async () =>
            {
                await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);
                await UnitOfWork.ChannelRepository.IncrementChannelMembersCountAsync(channel.Id);
            });
        }

        public async Task LeaveFromChannelAsync(string saasUserId, Guid channelId)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to leave from channel. Member {nameof(saasUserId)}:{saasUserId} is not found.");
            }

            var channel = await UnitOfWork.ChannelRepository.GetChannelAsync(channelId);
            if (channel == null)
            {
                throw new NetKitChatNotFoundException($"Unable to leave from channel. Channel {nameof(channelId)}:{channelId} is not found.");
            }
            if (channel.Type == ChannelType.Direct)
            {
                throw new NetKitChatInvalidOperationException($"Unable to leave from direct channel.");
            }

            var isMemberExistsInChannel = await UnitOfWork.ChannelRepository.IsMemberExistsInChannelAsync(member.Id, channelId);
            if (!isMemberExistsInChannel)
            {
                throw new NetKitChatInvalidOperationException($"Unable to leave from channel. Member {nameof(saasUserId)}:{saasUserId} is not joined to channel {nameof(channelId)}:{channelId}.");
            }

            await UnitOfWork.ExecuteTransactionAsync(async () =>
            {
                await UnitOfWork.ChannelMemberRepository.DeleteChannelMemberAsync(member.Id, channelId);
                await UnitOfWork.ChannelRepository.DecrementChannelMembersCountAsync(channelId);
            });
        }

        public async Task DeleteMemberFromChannelAsync(string saasUserId, Guid channelId, Guid memberToDeleteId)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to delete member from channel. Member {nameof(saasUserId)}:{saasUserId} is not found.");
            }

            if (member.Id == memberToDeleteId)
            {
                throw new NetKitChatNotFoundException($"Unable to delete member from channel. Unable to delete yourself. Use {nameof(LeaveFromChannelAsync)} method instead.");
            }

            var channel = await UnitOfWork.ChannelRepository.GetChannelAsync(channelId);
            if (channel == null)
            {
                throw new NetKitChatNotFoundException($"Unable to delete member from channel. Channel {nameof(channelId)}:{channelId} is not found.");
            }
            if (channel.Type == ChannelType.Direct)
            {
                throw new NetKitChatInvalidOperationException($"Unable to delete member from direct channel.");
            }
            if (member.Id != channel.CreatorId)
            {
                throw new NetKitChatAccessForbiddenException($"Unable to delete member from channel. Channel {nameof(channelId)}:{channelId} owner required.");
            }


            var isMemberExistsInChannel = await UnitOfWork.ChannelRepository.IsMemberExistsInChannelAsync(memberToDeleteId, channelId);
            if (!isMemberExistsInChannel)
            {
                throw new NetKitChatInvalidOperationException($"Unable to delete member from channel. Member {nameof(memberToDeleteId)}:{memberToDeleteId} is not joined to channel {nameof(channelId)}:{channelId}.");
            }

            await UnitOfWork.ExecuteTransactionAsync(async () =>
            {
                await UnitOfWork.ChannelMemberRepository.DeleteChannelMemberAsync(memberToDeleteId, channelId);
                await UnitOfWork.ChannelRepository.DecrementChannelMembersCountAsync(channelId);
            });
        }

        public async Task MuteChannelAsync(string saasUserId, Guid channelId, bool isMuted)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to mute channel. Member {nameof(saasUserId)}:{saasUserId} is not found.");
            }

            var isMemberExistsInChannel = await UnitOfWork.ChannelRepository.IsMemberExistsInChannelAsync(member.Id, channelId);
            if (!isMemberExistsInChannel)
            {
                throw new NetKitChatInvalidOperationException($"Unable to mute channel. Member {nameof(saasUserId)}:{saasUserId} is not joined channel {nameof(channelId)}:{channelId}.");
            }

            await UnitOfWork.ChannelMemberRepository.MuteChannelAsync(member.Id, channelId, isMuted);
        }

        public async Task PinChannelAsync(string saasUserId, Guid channelId, bool isPinned)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to pin channel. Member {nameof(saasUserId)}:{saasUserId} not found.");
            }

            var isMemberExistsInChannel = await UnitOfWork.ChannelRepository.IsMemberExistsInChannelAsync(member.Id, channelId);
            if (!isMemberExistsInChannel)
            {
                throw new NetKitChatInvalidOperationException($"Unable to pin channel. Member {nameof(saasUserId)}:{saasUserId} is not joined channel {nameof(channelId)}:{channelId}.");
            }

            await UnitOfWork.ChannelMemberRepository.PinChannelAsync(member.Id, channelId, isPinned);
        }

        public async Task<int> GetChannelMessagesCountAsync(Guid channelId)
        {
            return await UnitOfWork.MessageRepository.GetChannelMessagesCountAsync(channelId);
        }
    }
}
