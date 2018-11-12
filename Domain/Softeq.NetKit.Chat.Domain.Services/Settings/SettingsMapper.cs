// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using Softeq.NetKit.Chat.Domain.Settings.TransportModels.Response;

namespace Softeq.NetKit.Chat.Domain.Services.Settings
{
    internal static class SettingsMapper
    {
        public static SettingsResponse ToSettingsResponse(this Domain.Settings.Settings settings)
        {
            var settingsResponse = new SettingsResponse();
            if (settings != null)
            {
                settingsResponse.Id = settings.Id;
                settingsResponse.RawSettings = settings.RawSettings;
                settingsResponse.ChannelId = settings.ChannelId;
            }

            return settingsResponse;
        }
    }
}