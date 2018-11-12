// // Developed by Softeq Development Corporation
// // http://www.softeq.com

namespace Softeq.NetKit.Chat.Data.Interfaces.Repository.Common
{
    public interface IRepositoryFactory
    {
        IAttachmentRepository AttachmentRepository { get; }
        IClientRepository ClientRepository { get; }
        IMessageRepository MessageRepository { get; }
        INotificationRepository NotificationRepository { get; }
        IChannelMemberRepository ChannelMemberRepository { get; }
        IChannelRepository ChannelRepository { get; }
        ISettingRepository SettingRepository { get; }
        IMemberRepository MemberRepository { get; }
    }
}