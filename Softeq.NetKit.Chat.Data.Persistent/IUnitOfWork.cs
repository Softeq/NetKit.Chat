// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Data.Persistent.Repositories;

namespace Softeq.NetKit.Chat.Data.Persistent
{
    public interface IUnitOfWork
    {
        IAttachmentRepository AttachmentRepository { get; }
        IClientRepository ClientRepository { get; }
        IMessageRepository MessageRepository { get; }
        INotificationRepository NotificationRepository { get; }
        IChannelMemberRepository ChannelMemberRepository { get; }
        IChannelRepository ChannelRepository { get; }
        ISettingRepository SettingRepository { get; }
        IMemberRepository MemberRepository { get; }
        IForwardMessageRepository ForwardMessageRepository { get; }
        INotificationSettingRepository NotificationSettingRepository { get; }
    }
}