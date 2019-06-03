// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Data.SqlClient;
using Moq;
using Softeq.NetKit.Chat.Data.Cloud.DataProviders;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Data.Persistent.Sql;
using Softeq.NetKit.Chat.Data.Persistent.Sql.Database;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Domain.Services.Utility;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.ChannelService
{
    public abstract class ChannelServiceTestBase
    {
        protected readonly IChannelService _channelService;
        protected readonly Mock<IUnitOfWork> _unitOfWorkMock;

        protected readonly Mock<IMemberService> _memberServiceMock = new Mock<IMemberService>(MockBehavior.Strict);
        protected readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new Mock<IDateTimeProvider>(MockBehavior.Strict);
        protected readonly Mock<IDomainModelsMapper> _domainModelsMapperMock = new Mock<IDomainModelsMapper>(MockBehavior.Strict);
        protected readonly Mock<ISqlConnectionFactory> _sqlConnectionFactoryMock = new Mock<ISqlConnectionFactory>(MockBehavior.Strict);

        protected readonly Mock<IChannelRepository> _channelRepositoryMock = new Mock<IChannelRepository>(MockBehavior.Strict);
        protected readonly Mock<IMemberRepository> _memberRepositoryMock = new Mock<IMemberRepository>(MockBehavior.Strict);
        protected readonly Mock<IMessageRepository> _messageRepositoryMock = new Mock<IMessageRepository>(MockBehavior.Strict);
        protected readonly Mock<IForwardMessageRepository> _forwardMessageRepositoryMock = new Mock<IForwardMessageRepository>(MockBehavior.Strict);
        protected readonly Mock<IChannelMemberRepository> _channelMemberRepositoryMock = new Mock<IChannelMemberRepository>(MockBehavior.Strict);
        protected readonly Mock<IAttachmentRepository> _attachmentRepositoryMock = new Mock<IAttachmentRepository>(MockBehavior.Strict);
        protected readonly Mock<ISettingRepository> _settingRepositoryMock = new Mock<ISettingRepository>(MockBehavior.Strict);

        protected readonly Mock<ICloudImageProvider> _cloudImageProviderMock = new Mock<ICloudImageProvider>(MockBehavior.Strict);

        protected ChannelServiceTestBase()
        {
            _sqlConnectionFactoryMock.Setup(x => x.CreateConnection()).Returns(It.IsAny<SqlConnection>());
            _unitOfWorkMock = new Mock<UnitOfWork>(_sqlConnectionFactoryMock.Object, new TransactionConfiguration()).As<IUnitOfWork>();
            _unitOfWorkMock.CallBase = true;

            _unitOfWorkMock.Setup(x => x.ChannelRepository).Returns(_channelRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.MemberRepository).Returns(_memberRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.MessageRepository).Returns(_messageRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.ForwardMessageRepository).Returns(_forwardMessageRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.ChannelMemberRepository).Returns(_channelMemberRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.AttachmentRepository).Returns(_attachmentRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.SettingRepository).Returns(_settingRepositoryMock.Object);

            _channelService = new Chat.Domain.Services.DomainServices.ChannelService(
                _unitOfWorkMock.Object,
                _domainModelsMapperMock.Object,
                _memberServiceMock.Object,
                _cloudImageProviderMock.Object,
                _dateTimeProviderMock.Object);
        }

        protected void VerifyMocks()
        {
            _memberServiceMock.VerifyAll();
            _dateTimeProviderMock.VerifyAll();

            _domainModelsMapperMock.VerifyAll();

            _channelRepositoryMock.VerifyAll();
            _memberRepositoryMock.VerifyAll();
            _messageRepositoryMock.VerifyAll();
            _forwardMessageRepositoryMock.VerifyAll();
            _channelMemberRepositoryMock.VerifyAll();
            _attachmentRepositoryMock.VerifyAll();

            _cloudImageProviderMock.VerifyAll();

            _settingRepositoryMock.VerifyAll();
        }
    }
}