using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnsureThat;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Domain.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Settings;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Settings;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    internal class NotificationSettingsService : BaseService, INotificationSettingsService
    {
        private readonly IMemberService _memberService;

        public NotificationSettingsService(
            IUnitOfWork unitOfWork, 
            IDomainModelsMapper domainModelsMapper, 
            IMemberService memberService) : base(unitOfWork, domainModelsMapper)
        {
            _memberService = memberService;
        }

        public async Task<IList<string>> GetSaasUserIdsWithDisabledGroupNotificationsAsync()
        {
            return await UnitOfWork.NotificationSettingRepository.GetSaasUserIdsWithDisabledGroupNotificationsAsync();
        }

        public async Task<NotificationSettingResponse> UpdateUserNotificationSettingsAsync(NotificationSettingRequest notificationSettingRequest)
        {
            Ensure.That(notificationSettingRequest).IsNotNull();

            var member = await _memberService.GetMemberBySaasUserIdAsync(notificationSettingRequest.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException("Requested member does not exist.");
            }

            var existingNotificationSettings = await UnitOfWork.NotificationSettingRepository.GetSettingsByMemberIdAsync(member.Id);
            if (existingNotificationSettings != null)
            {
                existingNotificationSettings = FillNotificationSettings(notificationSettingRequest.Key, notificationSettingRequest.Value, existingNotificationSettings);
                await UnitOfWork.NotificationSettingRepository.UpdateSettingsAsync(existingNotificationSettings);
            }
            else
            {
                var notificationSettings = new NotificationSettings
                {
                    Id = Guid.NewGuid(),
                    MemberId = member.Id
                };

                notificationSettings = FillNotificationSettings(notificationSettingRequest.Key, notificationSettingRequest.Value, notificationSettings);
                await UnitOfWork.NotificationSettingRepository.AddSettingsAsync(notificationSettings);
            }

            var userNotificationSettings = await UnitOfWork.NotificationSettingRepository.GetSettingsByMemberIdAsync(member.Id);
            return DomainModelsMapper.MapToNotificationSettingsResponse(userNotificationSettings);
        }

        private NotificationSettings FillNotificationSettings(NotificationSettingKey notificationSettingKey, NotificationSettingValue notificationSettingValue, NotificationSettings notificationSettings)
        {
            switch (notificationSettingKey)
            {
                case NotificationSettingKey.GroupNotifications:
                {
                    notificationSettings.IsChannelNotificationsDisabled = notificationSettingValue;
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            return notificationSettings;
        }

        public async Task<NotificationSettingResponse> GetUserNotificationSettingsAsync(UserRequest userRequest)
        {
            var member = await _memberService.GetMemberBySaasUserIdAsync(userRequest.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException("Requested member does not exist.");
            }

            var userNotificationSettings = await UnitOfWork.NotificationSettingRepository.GetSettingsByMemberIdAsync(member.Id);
            if (userNotificationSettings == null)
            {
                return new NotificationSettingResponse
                {
                    MemberId = member.Id
                };
            }

            return DomainModelsMapper.MapToNotificationSettingsResponse(userNotificationSettings);
        }
    }
}
