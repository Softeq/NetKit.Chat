using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Softeq.NetKit.Chat.Notifications.Exceptions;
using Softeq.NetKit.Chat.Notifications.PushNotifications.Model;
using Softeq.NetKit.Chat.Notifications.TransportModels.Notification;

namespace Softeq.NetKit.Chat.Notifications.PushNotifications
{
    public class NotificationHub : INotificationHub
    {
        private NotificationHubClient _hubClient;

        public NotificationHub(IConfiguration configuration)
        {
            _hubClient = NotificationHubClient.CreateClientFromConnectionString(configuration["AzureNotificationHub:ConnectionString"], configuration["AzureNotificationHub:HubName"]);
        }

        public async Task DeleteAllRegistrationAsync(string tag)
        {
            try
            {
                var registrations = await _hubClient.GetRegistrationsByTagAsync(tag, int.MaxValue);
                foreach (RegistrationDescription registration in registrations)
                {
                    if (registration != null)
                    {
                        await _hubClient.DeleteRegistrationAsync(registration.RegistrationId);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new PushNotificationException("Something went wrong during deleting token request", ex);
            }
        }

        public async Task<IEnumerable<string>> GetAllRegistrationsAsync(string tag)
        {
            try
            {
                var registrations = await _hubClient.GetRegistrationsByTagAsync(tag, int.MaxValue);
                return registrations.FirstOrDefault()?.Tags;
            }
            catch (Exception ex)
            {
                throw new PushNotificationException("Something went wrong during getting push registration", ex);
            }
        }

        public async Task<IEnumerable<RegistrationDescription>> GetRegistrationsByTokenAsync(string deviceToken)
        {
            try
            {
                var registration = await _hubClient.GetRegistrationsByChannelAsync(deviceToken, int.MaxValue);
                return registration;
            }
            catch (Exception)
            {
                throw new PushNotificationException($"Something went wrong during getting registrations for deviceToken: {deviceToken}");
            }
        }

        public async Task DeleteRegistrationAsync(string deviceToken)
        {
            try
            {
                var registrations = await _hubClient.GetRegistrationsByChannelAsync(deviceToken, int.MaxValue);
                foreach (var registration in registrations)
                {
                    await _hubClient.DeleteRegistrationAsync(registration);
                }
            }
            catch (Exception)
            {
                throw new PushNotificationException("Something went wrong during deleting token request");
            }
        }

        public async Task<RegistrationDescription> CreateOrUpdateRegistrationAsync(string registrationId, DeviceRegistration deviceUpdate)
        {
            RegistrationDescription registration = null;
            switch (deviceUpdate.Platform)
            {
                case DevicePlatform.iOS:
                    registration = new AppleRegistrationDescription(deviceUpdate.DeviceToken)
                    {
                        DeviceToken = deviceUpdate.DeviceToken,
                        RegistrationId = registrationId,
                        Tags = new HashSet<string>(deviceUpdate.Tags)
                    };
                    break;
                case DevicePlatform.Android:
                    registration = new GcmRegistrationDescription(deviceUpdate.DeviceToken, deviceUpdate.Tags)
                    {
                        GcmRegistrationId = deviceUpdate.DeviceToken,
                        RegistrationId = registrationId
                    };
                    break;
                default:
                    throw new ArgumentException();
            }
            try
            {
                return await _hubClient.CreateOrUpdateRegistrationAsync(registration);
            }
            catch (Exception e)
            {
                throw new PushNotificationException("Error while trying to create or update notification hub", e);
            }
        }

        public async Task<bool> SendNotificationAsync(DevicePlatform platform, IPushNotificationModel model)
        {
            try
            {
                switch (platform)
                {
                    case DevicePlatform.iOS:
                        var data = JsonConvert.SerializeObject(model.GetData());
                        var iOSAlert = "{\"aps\":{\"alert\": { \"title\":\"" + model.Title + "\", \"body\":\"" + model.Body + "\"}, \"sound\":\"default\", \"badge\": " + model.Badge + ", \"type\": " + model.NotificationType + "}" + ", \"data\":" + data + " }";
                        await _hubClient.SendAppleNativeNotificationAsync(iOSAlert);
                        break;
                    case DevicePlatform.Android:
                        var androidAlert = JsonConvert.SerializeObject(new GcmNativeNotification()
                        {
                            Notification = new GcmNotificationData()
                            {
                                Title = model.Title,
                                Body = model.Body,
                                Sound = "default"
                            },
                            Data = new
                            {
                                type = model.NotificationType,
                                payload = model.GetData()
                            }
                        });
                        await _hubClient.SendGcmNativeNotificationAsync(androidAlert, model.RecipientIds);
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new PushNotificationException(ex.Message, ex);
            }
        }

        public async Task<string> CreateRegistrationId()
        {
            return await _hubClient.CreateRegistrationIdAsync();
        }


    }
}
