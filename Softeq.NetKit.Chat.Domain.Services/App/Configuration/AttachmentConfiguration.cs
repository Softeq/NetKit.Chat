// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.Extensions.Configuration;

namespace Softeq.NetKit.Chat.Domain.Services.App.Configuration
{
    internal class AttachmentConfiguration
    {
        public AttachmentConfiguration(IConfiguration configuration)
        {
            Limit = configuration.GetValue<int>("MessageAttachments:Limit");
        }

        public int Limit { get; }
    }
}