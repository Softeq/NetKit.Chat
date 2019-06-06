using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnsureThat;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Request;
using Softeq.NetKit.Services.PushNotifications.Abstractions;
using Softeq.NetKit.Services.PushNotifications.Models;

namespace Softeq.NetKit.Chat.Notifications.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly IMemberService _memberService;
        private readonly IChannelService _channelService;
        private readonly IPushNotificationSender _pushNotificationSender;
        private readonly IPushNotificationSubscriber _pushNotificationSubscriber;

        public PushNotificationService(
            IMemberService memberService,
            IChannelService channelService,
            IPushNotificationSender pushNotificationSender,
            IPushNotificationSubscriber pushNotificationSubscriber)
        {
            _memberService = memberService;
            _channelService = channelService;
            _pushNotificationSender = pushNotificationSender;
            _pushNotificationSubscriber = pushNotificationSubscriber;
        }

        public async Task UnsubscribeDeviceFromPushAsync(CreatePushTokenRequest model)
        {
            Ensure.That(model, nameof(model)).IsNotNull();
            await _pushNotificationSubscriber.UnsubscribeDeviceAsync(model.Token);
        }

        public async Task CreateOrUpdatePushSubscriptionAsync(CreatePushTokenRequest request)
        {
            Ensure.That(request, nameof(request)).IsNotNull();

            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);

            var subscribedChannels = await _channelService.GetAllowedChannelsAsync(request.SaasUserId);

            var tags = new List<string> { PushNotificationsTagTemplates.GetMemberSubscriptionTag(member.SaasUserId) };
            // add user subscribed channels tags
            tags.AddRange(subscribedChannels.Select(x => PushNotificationsTagTemplates.GetChatChannelTag(x.Id.ToString())));

            await _pushNotificationSubscriber.CreateOrUpdatePushSubscriptionAsync(new PushSubscriptionRequest(request.Token, request.DevicePlatform, tags));
        }

        public async Task SubscribeUserOnTagsAsync(string userId, IEnumerable<string> tagNames)
        {
            var registrations = await _pushNotificationSubscriber.GetRegistrationsByTagAsync(PushNotificationsTagTemplates.GetMemberSubscriptionTag(userId));

            foreach (var registration in registrations)
            {
                bool hasAnyNotIncludedTags = false;
                var notIncludedTags = tagNames.Where(x => !registration.Tags.Contains(x));

                foreach (var notIncludedTag in notIncludedTags)
                {
                    registration.Tags.Add(notIncludedTag);
                    hasAnyNotIncludedTags = true;
                }

                if (hasAnyNotIncludedTags)
                {
                    if (registration.Platform == PushPlatformEnum.iOS)
                    {
                        await _pushNotificationSubscriber.CreateOrUpdatePushSubscriptionAsync(new PushSubscriptionRequest(registration.PnsHandle, PushPlatformEnum.iOS, registration.Tags));
                    }
                    else if (registration.Platform == PushPlatformEnum.Android)
                    {
                        await _pushNotificationSubscriber.CreateOrUpdatePushSubscriptionAsync(new PushSubscriptionRequest(registration.PnsHandle, PushPlatformEnum.Android, registration.Tags));
                    }
                }
            }
        }

        public async Task SubscribeUserOnTagAsync(string userId, string tagName)
        {
            var registrations = await _pushNotificationSubscriber.GetRegistrationsByTagAsync(PushNotificationsTagTemplates.GetMemberSubscriptionTag(userId));

            foreach (var registration in registrations)
            {
                if (!registration.Tags.Contains(tagName))
                {
                    registration.Tags.Add(tagName);

                    if (registration.Platform == PushPlatformEnum.iOS)
                    {
                        await _pushNotificationSubscriber.CreateOrUpdatePushSubscriptionAsync(new PushSubscriptionRequest(registration.PnsHandle, PushPlatformEnum.iOS, registration.Tags));
                    }
                    else if (registration.Platform == PushPlatformEnum.Android)
                    {
                        await _pushNotificationSubscriber.CreateOrUpdatePushSubscriptionAsync(new PushSubscriptionRequest(registration.PnsHandle, PushPlatformEnum.Android, registration.Tags));
                    }
                }
            }
        }

        public async Task UnsubscribeUserFromTagAsync(string userId, string tagName)
        {
            var registrations = await _pushNotificationSubscriber.GetRegistrationsByTagAsync(PushNotificationsTagTemplates.GetMemberSubscriptionTag(userId));

            foreach (var registration in registrations)
            {
                if (registration.Tags.Contains(tagName))
                {
                    registration.Tags.Remove(tagName);

                    if (registration.Platform == PushPlatformEnum.iOS)
                    {
                        await _pushNotificationSubscriber.CreateOrUpdatePushSubscriptionAsync(new PushSubscriptionRequest(registration.PnsHandle, PushPlatformEnum.iOS, registration.Tags));
                    }
                    else if (registration.Platform == PushPlatformEnum.Android)
                    {
                        await _pushNotificationSubscriber.CreateOrUpdatePushSubscriptionAsync(new PushSubscriptionRequest(registration.PnsHandle, PushPlatformEnum.Android, registration.Tags));
                    }
                }
            }
        }

        public async Task<bool> SendToSingleAsync(string tag, PushNotificationMessage model)
        {
            return await _pushNotificationSender.SendAsync(model, tag);
        }

        public async Task<bool> SendForTagAsync(PushNotificationMessage model, List<string> includedTags, List<string> excludedTags)
        {
            return await _pushNotificationSender.SendAsync(model, includedTags, excludedTags);
        }
    }
}
