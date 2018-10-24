// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.Services.App.Configuration
{
    internal class AttachmentConfiguration
    {
        public AttachmentConfiguration(int limit)
        {
            Limit = limit;
        }

        public int Limit { get; set; }
    }
}