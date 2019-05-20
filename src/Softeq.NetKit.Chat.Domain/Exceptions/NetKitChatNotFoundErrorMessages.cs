// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    public static class NetKitChatNotFoundErrorMessages
    {
        public static string UnableGetMembersCauseNoChannel => "Unable to get channel members. Channel {0} is not found.";
        public static string UnableGetClient => "Unable to get client. Client {0} is not found.";
        public static string UnableGetMember => "Unable to get member by {0}. Member {1} is not found.";
        public static string UnableGetMemberClients => "Unable to get member clients. Member {0} is not found.";
        public static string UnableGetMessage => "Unable to get message by {0}. Message {1} is not found.";
        public static string UnableGetCahnnel => "Unable to get channel by {0}. Channel {1} is not found.";
        public static string UnableGetMemberChannels => "Unable to get member channels. Member {0} is not found.";

        public static string UnableAddMember => "Unable to add member to channel. Member {0} not found.";

        public static string NoRequestedMember => "Requested member does not exist.";

        public static string UnableDeleteClient => "Unable to delete client. Client {0} is not found.";
        public static string UnableDeleteMessage => "Unable to delete message. Message {0} is not found.";
        public static string UnableDeleteMessageAttachmentCauseNoMessage => "Unable to delete message attachment. Message {0} is not found.";
        public static string UnableDeleteMessageAttachmentCauseNoMember => "Unable to delete message attachment. Member {0} is not found.";
        public static string UnableDeleteMessageCauseNoMember => "Unable to delete message. Member {0} is not found.";
        public static string UnableDeleteMessageAttachment => "Unable to delete message attachment. Attachment {0} is not found.";
    
        public static string UnableInviteMemberCauseNoChannel => "Unable to invite member. Channel {0} is not found.";
        public static string UnableInviteMember => "Unable to invite member. Member {0} is not found.";

        public static string UnableUpdateMemberStatus => "Unable to update member status. Member {0} is not found.";
        public static string UnableUpdateActivityCauseNoMember => "Unable to update activity. Member {0} is not found.";
        public static string UnableUpdateActivityCauseNoClient => "Unable to update activity. Client {0} is not found.";
        public static string UnableUpdateMessage => "Unable to update message. Message {0} is not found.";
        public static string UnableUpdateMessageCauseNoMember => "Unable to update message. Member {0} is not found.";

        public static string UnableCreateMessageCauseNoChannel => "Unable to create message. Channel {0} is not found.";
        public static string UnableCreateMessageCauseNoForwardMessasge => "Unable to create message. Forward message {0} is not found.";
        public static string UnableCreateChannelCauseNoMember => "Unable to create channel. Member with {0} is not found.";
        public static string UnableCreateMessageNoMember => "Unable to create message. Member {0} is not found.";

        public static string UnableAddMessageAttachmentCauseNoMessage =>"Unable to add message attachment. Message {0} is not found.";
        public static string UnableAddMessageAttachmentCauseNoMember => "Unable to add message attachment. Member {0} is not found.";
    }
}
