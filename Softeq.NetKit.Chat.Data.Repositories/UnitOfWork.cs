// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using Softeq.NetKit.Chat.Data.Interfaces.Repository;
using Softeq.NetKit.Chat.Data.Interfaces.UnitOfWork;
using Softeq.NetKit.Chat.Data.Repositories.Infrastructure;
using Softeq.NetKit.Chat.Data.Repositories.Repositories;

namespace Softeq.NetKit.Chat.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        private IAttachmentRepository _attachmentRepository;

        private IChannelMemberRepository _channelMemberRepository;

        private IChannelRepository _channelRepository;
        private IClientRepository _clientRepository;

        private IMemberRepository _memberRepository;

        private IMessageRepository _messageRepository;

        private INotificationRepository _notificationRepository;

        private ISettingRepository _settingRepository;

        public UnitOfWork(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public IAttachmentRepository AttachmentRepository =>
            _attachmentRepository ?? (_attachmentRepository = new AttachmentRepository(_sqlConnectionFactory));

        public IClientRepository ClientRepository =>
            _clientRepository ?? (_clientRepository = new ClientRepository(_sqlConnectionFactory));

        public IMessageRepository MessageRepository =>
            _messageRepository ?? (_messageRepository = new MessageRepository(_sqlConnectionFactory));

        public INotificationRepository NotificationRepository =>
            _notificationRepository ?? (_notificationRepository = new NotificationRepository(_sqlConnectionFactory));

        public IChannelMemberRepository ChannelMemberRepository =>
            _channelMemberRepository ?? (_channelMemberRepository = new ChannelMemberRepository(_sqlConnectionFactory));

        public IChannelRepository ChannelRepository =>
            _channelRepository ?? (_channelRepository = new ChannelRepository(_sqlConnectionFactory));

        public ISettingRepository SettingRepository =>
            _settingRepository ?? (_settingRepository = new SettingRepository(_sqlConnectionFactory));

        public IMemberRepository MemberRepository =>
            _memberRepository ?? (_memberRepository = new MemberRepository(_sqlConnectionFactory));
    }
}