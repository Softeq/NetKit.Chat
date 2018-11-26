using System.Linq;
using Softeq.NetKit.Chat.Tests.Abstract;
using Softeq.NetKit.Chat.Notifications;
using Xunit;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Configuration;
using Softeq.NetKit.Chat.Notifications.PushNotifications;
using Softeq.NetKit.Chat.Notifications.TransportModels.Notification;

namespace Softeq.NetKit.Chat.Tests.NotificationTests
{
    public class NotificationHubTests : BaseTest
    {
        private INotificationHub _notificationHub;

        public NotificationHubTests()
        {
            _notificationHub = new NotificationHub(LifetimeScope.Resolve<IConfiguration>());
        }
        
        private const string _deviceToken = "3cb4a234-20b2-4568-9684-c3213cca7788";
        private const string _saasUserId = "2ae3a155-10b1-4c69-8573-d4527aba8860";

        [Fact]
        public async Task CreateOrUpdateRegistrationAsync_ShouldCreateOneRegistration()
        {
            var registerId = await _notificationHub.CreateRegistrationId();

            DeviceRegistration deviceRegistration = new DeviceRegistration()
            {
                DeviceToken = _deviceToken,
                Platform = DevicePlatform.Android,
                Tags = new []{_saasUserId}
            };

            await _notificationHub.DeleteAllRegistrationAsync(_saasUserId);
            await _notificationHub.CreateOrUpdateRegistrationAsync(registerId, deviceRegistration);
            var allRegistrationsAfterCreating = await _notificationHub.GetAllRegistrationsAsync(_saasUserId);

            allRegistrationsAfterCreating.Should().NotBeEmpty();
        }
    }
}
