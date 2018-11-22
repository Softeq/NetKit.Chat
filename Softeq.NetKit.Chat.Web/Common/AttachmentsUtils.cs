// Developed by Softeq Development Corporation
// http://www.softeq.com
namespace Softeq.NetKit.Chat.Web.Common
{
    public static class AttachmentsUtils
    {
        public static string GetExtensionFromMimeType(string mimeType)
        {
            switch (mimeType)
            {
                case "image/jpeg": return "jpg";
                case "image/png": return "png";
                case "video/mp4": return "mp4";
                default: return null;
            }
        }
    }
}
