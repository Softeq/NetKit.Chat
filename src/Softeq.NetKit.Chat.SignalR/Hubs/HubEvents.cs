// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.SignalR.Hubs
{
    public static class HubEvents
    {
        public const string MessageDeleted = "MessageDeleted";
        public const string MessageAdded = "MessageAdded";
        public const string MessageUpdated = "MessageUpdated";
        public const string LastReadMessageChanged = "LastReadMessageChanged";

        public const string AttachmentAdded = "AttachmentAdded";
        public const string AttachmentDeleted = "AttachmentDeleted";

        public const string MemberLeft = "MemberLeft";
        public const string MemberJoined = "MemberJoined";

        public const string ChannelClosed = "ChannelClosed";
        public const string ChannelUpdated = "ChannelUpdated";

        public const string ExceptionOccurred = "ExceptionOccurred";
        public const string RequestSuccess = "RequestSuccess";

        public const string RequestValidationFailed = "RequestValidationFailed";
    }
}