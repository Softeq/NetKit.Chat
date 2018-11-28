// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.IO;
using System.Threading.Tasks;
using Softeq.CloudStorage.Extension;
using Softeq.NetKit.Chat.Data.Cloud.Azure.Configuration;
using Softeq.NetKit.Chat.Data.Cloud.DataProviders;

namespace Softeq.NetKit.Chat.Data.Cloud.Azure.DataProviders
{
    internal class CloudAttachmentProvider : BaseCloudProvider, ICloudAttachmentProvider
    {
        public CloudAttachmentProvider(IContentStorage storage, AzureStorageConfiguration configuration)
            : base(storage, configuration)
        {
        }

        public async Task DeleteMessageAttachmentAsync(string attachmentFileName)
        {
            await Storage.DeleteContentAsync(attachmentFileName, Configuration.MessageAttachmentsContainer);
        }

        public async Task<string> SaveAttachmentAsync(string attachmentFileName, Stream content)
        {
            return await Storage.SaveContentAsync(attachmentFileName, content, Configuration.MessageAttachmentsContainer);
        }
    }
}