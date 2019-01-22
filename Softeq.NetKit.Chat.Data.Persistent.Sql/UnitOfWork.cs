// Developed by Softeq Development Corporation
// http://www.softeq.com

using EnsureThat;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Data.Persistent.Sql.Database;
using Softeq.NetKit.Chat.Data.Persistent.Sql.Repositories;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public UnitOfWork(ISqlConnectionFactory sqlConnectionFactory)
        {
            Ensure.That(sqlConnectionFactory).IsNotNull();

            _sqlConnectionFactory = sqlConnectionFactory;
        }

        private IAttachmentRepository _attachmentRepository;
        public IAttachmentRepository AttachmentRepository => _attachmentRepository ?? (_attachmentRepository = new AttachmentRepository(_sqlConnectionFactory));

        private IClientRepository _clientRepository;
        public IClientRepository ClientRepository => _clientRepository ?? (_clientRepository = new ClientRepository(_sqlConnectionFactory));

        private IMessageRepository _messageRepository;
        public IMessageRepository MessageRepository => _messageRepository ?? (_messageRepository = new MessageRepository(_sqlConnectionFactory));

        private INotificationRepository _notificationRepository;
        public INotificationRepository NotificationRepository => _notificationRepository ?? (_notificationRepository = new NotificationRepository(_sqlConnectionFactory));

        private IChannelMemberRepository _channelMemberRepository;
        public IChannelMemberRepository ChannelMemberRepository => _channelMemberRepository ?? (_channelMemberRepository = new ChannelMemberRepository(_sqlConnectionFactory));

        private IChannelRepository _channelRepository;
        public IChannelRepository ChannelRepository => _channelRepository ?? (_channelRepository = new ChannelRepository(_sqlConnectionFactory));

        private ISettingRepository _settingRepository;
        public ISettingRepository SettingRepository => _settingRepository ?? (_settingRepository = new SettingRepository(_sqlConnectionFactory));

        private IMemberRepository _memberRepository;
        public IMemberRepository MemberRepository => _memberRepository ?? (_memberRepository = new MemberRepository(_sqlConnectionFactory));

        private IForwardMessageRepository _forwardMessageRepository;
        public IForwardMessageRepository ForwardMessageRepository => _forwardMessageRepository ?? (_forwardMessageRepository = new ForwardMessageRepository(_sqlConnectionFactory));

        private IDirectMemberRepository _directMemberRepository;
        public IDirectMemberRepository DirectMemberRepository => _directMemberRepository ?? (_directMemberRepository = new DirectMembersRepository(_sqlConnectionFactory));
    }
}