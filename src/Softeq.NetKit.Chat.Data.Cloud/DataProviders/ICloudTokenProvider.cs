// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.Data.Cloud.DataProviders
{
    public interface ICloudTokenProvider
    {
        Task<string> GetTemporaryStorageAccessTokenAsync(int expirationTimeMinutes);
    }
}