// Developed by Softeq Development Corporation
// http://www.softeq.com

using Moq;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Domain.Services.Utility;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.DirectChannelService
{
    public abstract class DirectChannelTestBase
    {
        protected readonly IDirectChannelService DirectChannelService;

        protected readonly Mock<IDomainModelsMapper> _domainModelsMapperMock = new Mock<IDomainModelsMapper>(MockBehavior.Strict);

        protected readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);

        protected readonly Mock<IChannelRepository> _channelRepositoryMock = new Mock<IChannelRepository>(MockBehavior.Strict);
        protected readonly Mock<IMemberRepository> _memberRepositoryMock = new Mock<IMemberRepository>(MockBehavior.Strict);
        protected readonly Mock<IMessageRepository> _messageRepositoryMock = new Mock<IMessageRepository>(MockBehavior.Strict);
        protected readonly Mock<IForwardMessageRepository> _forwardMessageRepositoryMock = new Mock<IForwardMessageRepository>(MockBehavior.Strict);
        protected readonly Mock<IChannelMemberRepository> _channelMemberRepositoryMock = new Mock<IChannelMemberRepository>(MockBehavior.Strict);
        protected readonly Mock<IAttachmentRepository> _attachmentRepositoryMock = new Mock<IAttachmentRepository>(MockBehavior.Strict);
        protected readonly Mock<IClientRepository> _clientRepositoryMock = new Mock<IClientRepository>(MockBehavior.Strict);
        protected readonly Mock<INotificationRepository> _notificationRepositoryMock = new Mock<INotificationRepository>(MockBehavior.Strict);
        protected readonly Mock<IDirectChannelRepository> _directChannelRepositoryMock = new Mock<IDirectChannelRepository>(MockBehavior.Strict);
        protected readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new Mock<IDateTimeProvider>(MockBehavior.Strict);

        protected DirectChannelTestBase()
        {
            _unitOfWorkMock.Setup(x => x.ChannelRepository).Returns(_channelRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.MemberRepository).Returns(_memberRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.MessageRepository).Returns(_messageRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.ForwardMessageRepository).Returns(_forwardMessageRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.ChannelMemberRepository).Returns(_channelMemberRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.AttachmentRepository).Returns(_attachmentRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.ClientRepository).Returns(_clientRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.NotificationRepository).Returns(_notificationRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.DirectChannelRepository).Returns(_directChannelRepositoryMock.Object);
 
            DirectChannelService = new Chat.Domain.Services.DomainServices.DirectChannelService(_unitOfWorkMock.Object,
                _domainModelsMapperMock.Object, _dateTimeProviderMock.Object);
        }

        protected void VerifyMocks()
        {
            _directChannelRepositoryMock.VerifyAll();
            _domainModelsMapperMock.VerifyAll();
            _channelRepositoryMock.VerifyAll();
            _memberRepositoryMock.VerifyAll();
            _messageRepositoryMock.VerifyAll();
            _forwardMessageRepositoryMock.VerifyAll();
            _channelMemberRepositoryMock.VerifyAll();
            _attachmentRepositoryMock.VerifyAll();
            _clientRepositoryMock.VerifyAll();
            _notificationRepositoryMock.VerifyAll();
            _dateTimeProviderMock.VerifyAll();
        }
    }
}
