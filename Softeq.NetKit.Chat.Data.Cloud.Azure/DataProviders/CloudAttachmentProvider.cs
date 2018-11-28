// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.IO;
using System.Threading.Tasks;
using EnsureThat;
using Softeq.CloudStorage.Extension;
using Softeq.NetKit.Chat.Data.Cloud.Azure.Configuration;
using Softeq.NetKit.Chat.Data.Cloud.DataProviders;

namespace Softeq.NetKit.Chat.Data.Cloud.Azure.DataProviders
{
    internal class CloudAttachmentProvider : ICloudAttachmentProvider
    {
        private readonly IContentStorage _contentStorage;
        private readonly AzureStorageConfiguration _storageConfiguration;

        public CloudAttachmentProvider(IContentStorage contentStorage, AzureStorageConfiguration storageConfiguration)
        {
            Ensure.That(contentStorage).IsNotNull();
            Ensure.That(storageConfiguration).IsNotNull();

            _contentStorage = contentStorage;
            _storageConfiguration = storageConfiguration;
        }

        public async Task DeleteMessageAttachmentAsync(string attachmentFileName)
        {
            await _contentStorage.DeleteContentAsync(attachmentFileName, _storageConfiguration.MessageAttachmentsContainer);
        }

        public async Task<string> SaveAttachmentAsync(string attachmentFileName, Stream content)
        {
            return await _contentStorage.SaveContentAsync(attachmentFileName, content, _storageConfiguration.MessageAttachmentsContainer);
        }
    }
}