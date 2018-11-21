// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Infrastructure.Storage.Sql.Repositories;

namespace Softeq.NetKit.Chat.Infrastructure.Storage.Sql
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