// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.Data.Cloud.DataProviders
{
    public interface ICloudImageProvider
    {
        Task<string> CopyImageToDestinationContainerAsync(string photoUrl);

        string GetMemberAvatarUrl(string memberPhotoName);
    }
}