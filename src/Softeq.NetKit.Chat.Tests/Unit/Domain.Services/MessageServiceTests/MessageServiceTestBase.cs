// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Microsoft.Extensions.Configuration;
using Moq;
using Softeq.NetKit.Chat.Data.Cloud.DataProviders;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Domain.Services.Configuration;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Domain.Services.Utility;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MessageServiceTests
{
    public abstract class MessageServiceTestBase
    {
        protected const int MessageAttachmentsLimit = 10;
        protected const int LastMessageReadCount = 20;

        protected readonly IMessageService _messageService;

        protected readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new Mock<IDateTimeProvider>(MockBehavior.Strict);

        protected readonly Mock<IDomainModelsMapper> _domainModelsMapperMock = new Mock<IDomainModelsMapper>(MockBehavior.Strict);

        protected readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        protected readonly Mock<IChannelRepository> _channelRepositoryMock = new Mock<IChannelRepository>(MockBehavior.Strict);
        protected readonly Mock<IMemberRepository> _memberRepositoryMock = new Mock<IMemberRepository>(MockBehavior.Strict);
        protected readonly Mock<IMessageRepository> _messageRepositoryMock = new Mock<IMessageRepository>(MockBehavior.Strict);
        protected readonly Mock<IForwardMessageRepository> _forwardMessageRepositoryMock = new Mock<IForwardMessageRepository>(MockBehavior.Strict);
        protected readonly Mock<IChannelMemberRepository> _channelMemberRepositoryMock = new Mock<IChannelMemberRepository>(MockBehavior.Strict);
        protected readonly Mock<IAttachmentRepository> _attachmentRepositoryMock = new Mock<IAttachmentRepository>(MockBehavior.Strict);

        protected readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>(MockBehavior.Strict);
        
        protected readonly Mock<ICloudAttachmentProvider> _cloudAttachmentProviderMock = new Mock<ICloudAttachmentProvider>(MockBehavior.Strict);
        protected readonly Mock<ICloudImageProvider> _cloudImageProviderMock = new Mock<ICloudImageProvider>(MockBehavior.Strict);

        protected MessageServiceTestBase()
        {
            _unitOfWorkMock.Setup(x => x.ChannelRepository).Returns(_channelRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.MemberRepository).Returns(_memberRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.MessageRepository).Returns(_messageRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.ForwardMessageRepository).Returns(_forwardMessageRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.ChannelMemberRepository).Returns(_channelMemberRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.AttachmentRepository).Returns(_attachmentRepositoryMock.Object);

            var messageAttachmentsLimitSectionMock = new Mock<IConfigurationSection>();
            messageAttachmentsLimitSectionMock.Setup(x => x.Value).Returns(MessageAttachmentsLimit.ToString());
            _configurationMock.Setup(x => x.GetSection("Message:MessageAttachmentsLimit")).Returns(messageAttachmentsLimitSectionMock.Object);

            var lastMessageReadCountSectionMock = new Mock<IConfigurationSection>();
            lastMessageReadCountSectionMock.Setup(x => x.Value).Returns(LastMessageReadCount.ToString());
            _configurationMock.Setup(x => x.GetSection("Message:LastMessageReadCount")).Returns(lastMessageReadCountSectionMock.Object);

            var attachmentConfiguration = new MessagesConfiguration(MessageAttachmentsLimit, LastMessageReadCount);

            _messageService = new MessageService(
                _unitOfWorkMock.Object, 
                _domainModelsMapperMock.Object, 
                attachmentConfiguration, 
                _cloudAttachmentProviderMock.Object,
                _cloudImageProviderMock.Object,
                _dateTimeProviderMock.Object);
        }

        protected void VerifyMocks()
        {
            _dateTimeProviderMock.VerifyAll();

            _domainModelsMapperMock.VerifyAll();

            _channelRepositoryMock.VerifyAll();
            _memberRepositoryMock.VerifyAll();
            _messageRepositoryMock.VerifyAll();
            _forwardMessageRepositoryMock.VerifyAll();
            _channelMemberRepositoryMock.VerifyAll();
            _attachmentRepositoryMock.VerifyAll();

            _cloudAttachmentProviderMock.VerifyAll();
            _cloudImageProviderMock.VerifyAll();
        }
    }
}
