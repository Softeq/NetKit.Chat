// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.IO;
using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.Data.Cloud.DataProviders
{
    public interface ICloudAttachmentProvider
    {
        Task DeleteMessageAttachmentAsync(string attachmentFileName);

        Task<string> SaveAttachmentAsync(string attachmentFileName, Stream content);
    }
}