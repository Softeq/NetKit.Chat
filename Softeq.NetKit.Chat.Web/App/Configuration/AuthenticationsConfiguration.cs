// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Web.App.Configuration
{
    public class AuthenticationsConfiguration
    {
        public BearerConfiguration Bearer { get; set; }

        public struct BearerConfiguration
        {
            public string ApiSecret { get; set; }

            public string Authority { get; set; }

            public bool RequireHttpsMetadata { get; set; }

            public string ApiName { get; set; }
        }
    }
}
