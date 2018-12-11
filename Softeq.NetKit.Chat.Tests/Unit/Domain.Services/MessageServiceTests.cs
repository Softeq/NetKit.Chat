// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Softeq.NetKit.Chat.Data.Cloud.DataProviders;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Configuration;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Domain.Services.Utility;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services
{
    public class MessageServiceTests
    {
        private readonly IMessageService _messageService;

        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new Mock<IDateTimeProvider>(MockBehavior.Strict);

        private readonly Mock<IDomainModelsMapper> _domainModelsMapperMock = new Mock<IDomainModelsMapper>(MockBehavior.Strict);

        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        private readonly Mock<IChannelRepository> _channelRepositoryMock = new Mock<IChannelRepository>(MockBehavior.Strict);
        private readonly Mock<IMemberRepository> _memberRepositoryMock = new Mock<IMemberRepository>(MockBehavior.Strict);
        private readonly Mock<IMessageRepository> _messageRepositoryMock = new Mock<IMessageRepository>(MockBehavior.Strict);
        private readonly Mock<IForwardMessageRepository> _forwardMessageRepositoryMock = new Mock<IForwardMessageRepository>(MockBehavior.Strict);
        private readonly Mock<IChannelMemberRepository> _channelMemberRepositoryMock = new Mock<IChannelMemberRepository>(MockBehavior.Strict);
        private readonly Mock<IAttachmentRepository> _attachmentRepositoryMock = new Mock<IAttachmentRepository>(MockBehavior.Strict);

        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>(MockBehavior.Strict);
        private readonly Mock<IConfigurationSection> _configurationSectionMock = new Mock<IConfigurationSection>();

        private readonly Mock<ICloudImageProvider> _cloudImageProviderMock = new Mock<ICloudImageProvider>(MockBehavior.Strict);
        private readonly Mock<ICloudAttachmentProvider> _cloudAttachmentProviderMock = new Mock<ICloudAttachmentProvider>(MockBehavior.Strict);

        public MessageServiceTests()
        {
            _unitOfWorkMock.Setup(x => x.ChannelRepository).Returns(_channelRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.MemberRepository).Returns(_memberRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.MessageRepository).Returns(_messageRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.ForwardMessageRepository).Returns(_forwardMessageRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.ChannelMemberRepository).Returns(_channelMemberRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.AttachmentRepository).Returns(_attachmentRepositoryMock.Object);

            _configurationMock.Setup(x => x.GetSection(It.IsAny<string>())).Returns(_configurationSectionMock.Object);
            var attachmentConfiguration = new AttachmentConfiguration(_configurationMock.Object);

            _messageService = new MessageService(_unitOfWorkMock.Object, _domainModelsMapperMock.Object, attachmentConfiguration, _cloudAttachmentProviderMock.Object, _dateTimeProviderMock.Object);
        }

        [Fact]
        public void CreateMessageAsync_ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            _channelRepositoryMock.Setup(x => x.IsChannelExistsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false)
                .Verifiable();

            var request = new CreateMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), MessageType.Default, "body");

            // Act
            Func<Task> act = async () => { await _messageService.CreateMessageAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to create message. Channel {nameof(request.ChannelId)}:{request.ChannelId} not found.");

            VerifyMocks();
        }

        [Fact]
        public void CreateMessageAsync_ShouldThrowIfChannelDoesNotExist()
        {
            // Arrange
            _channelRepositoryMock.Setup(x => x.IsChannelExistsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true)
                .Verifiable();
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            var request = new CreateMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), MessageType.Default, "body");

            // Act
            Func<Task> act = async () => { await _messageService.CreateMessageAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to create message. Member {nameof(request.SaasUserId)}:{request.SaasUserId} not found.");

            VerifyMocks();
        }

        [Fact]
        public void CreateMessageAsync_ShouldThrowIfForwardMessageDoesNotExist()
        {
            // Arrange
            var request = new CreateMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), MessageType.Forward, "body")
            {
                ForwardedMessageId = new Guid("1325E3C3-7D3E-4502-9CE8-7D7A902C90EE")
            };

            _channelRepositoryMock.Setup(x => x.IsChannelExistsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true)
                .Verifiable();

            var member = new Member { Id = new Guid("20CAE535-7CE3-4ED4-9D8F-2693953E94D3") };
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(member)
                .Verifiable();

            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.Is<Guid>(messageId => messageId.Equals(request.ForwardedMessageId))))
                .ReturnsAsync((Message)null)
                .Verifiable();

            _dateTimeProviderMock.Setup(x => x.GetUtcNow())
                .Returns(DateTimeOffset.UtcNow)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await _messageService.CreateMessageAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to create message. Forward message {nameof(request.ForwardedMessageId)}:{request.ForwardedMessageId} not found.");

            VerifyMocks();
        }

        [Fact]
        public async Task CreateMessageAsync_ShouldCreateDefaultMessage()
        {
            // Arrange
            var request = new CreateMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), MessageType.Default, "body")
            {
                ImageUrl = "ImageUrl"
            };

            _channelRepositoryMock.Setup(x => x.IsChannelExistsAsync(It.Is<Guid>(channelId => channelId.Equals(request.ChannelId))))
                .ReturnsAsync(true)
                .Verifiable();

            var member = new Member { Id = new Guid("20CAE535-7CE3-4ED4-9D8F-2693953E94D3") };
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasUserId => saasUserId.Equals(request.SaasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            Message message = null;
            _messageRepositoryMock.Setup(x => x.AddMessageAsync(It.IsAny<Message>()))
                .Callback<Message>(x => message = x)
                .Returns(Task.CompletedTask)
                .Verifiable();

            _channelMemberRepositoryMock.Setup(x => x.SetLastReadMessageAsync(
                    It.Is<Guid>(memberId => memberId.Equals(member.Id)),
                    It.Is<Guid>(channelId => channelId.Equals(request.ChannelId)),
                    It.IsAny<Guid>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var messageId = Guid.Empty;
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .Callback<Guid>(x => messageId = x)
                .ReturnsAsync((Message)null)
                .Verifiable();

            var utcNow = DateTimeOffset.UtcNow;
            _dateTimeProviderMock.Setup(x => x.GetUtcNow())
                .Returns(utcNow)
                .Verifiable();

            var messageResponse = new MessageResponse();
            _domainModelsMapperMock.Setup(x => x.MapToMessageResponse(It.IsAny<Message>(), It.IsAny<DateTimeOffset?>()))
                .Returns(messageResponse)
                .Verifiable();

            // Act
            await _messageService.CreateMessageAsync(request);

            // Assert
            VerifyMocks();

            message.Id.Should().Be(messageId);
            message.ChannelId.Should().Be(request.ChannelId);
            message.OwnerId.Should().Be(member.Id);
            message.Body.Should().Be(request.Body);
            message.Type.Should().Be(request.Type);
            message.ImageUrl.Should().Be(request.ImageUrl);
            message.Created.Should().Be(utcNow);
        }

        [Fact]
        public async Task CreateMessageAsync_ShouldCreateForwardMessage()
        {
            // Arrange
            var request = new CreateMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), MessageType.Forward, "body")
            {
                ForwardedMessageId = new Guid("D51A3CC1-80F8-41F4-9291-54372B8A7DAE"),
                ImageUrl = "ImageUrl"
            };

            _channelRepositoryMock.Setup(x => x.IsChannelExistsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true)
                .Verifiable();

            var member = new Member { Id = new Guid("20CAE535-7CE3-4ED4-9D8F-2693953E94D3") };
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(member)
                .Verifiable();

            Message message = null;
            _messageRepositoryMock.Setup(x => x.AddMessageAsync(It.IsAny<Message>()))
                .Callback<Message>(x => message = x)
                .Returns(Task.CompletedTask)
                .Verifiable();

            _channelMemberRepositoryMock.Setup(x => x.SetLastReadMessageAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => id == request.ForwardedMessageId ? new Message { Id = id } : null)
                .Verifiable();

            var forwardMessageId = Guid.Empty;
            _forwardMessageRepositoryMock.Setup(x => x.AddForwardMessageAsync(It.IsAny<ForwardMessage>()))
                .Callback<ForwardMessage>(x => forwardMessageId = x.Id)
                .Returns(Task.CompletedTask)
                .Verifiable();

            _dateTimeProviderMock.Setup(x => x.GetUtcNow())
                .Returns(DateTimeOffset.UtcNow)
                .Verifiable();
            
            _domainModelsMapperMock.Setup(x => x.MapToMessageResponse(It.IsAny<Message>(), It.IsAny<DateTimeOffset?>()))
                .Returns(new MessageResponse())
                .Verifiable();
            
            _domainModelsMapperMock.Setup(x => x.MapToForwardMessage(It.IsAny<Message>()))
                .Returns(new ForwardMessage())
                .Verifiable();

            // Act
            await _messageService.CreateMessageAsync(request);

            // Assert
            VerifyMocks();
            _messageRepositoryMock.Verify(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()), Times.Exactly(2));

            forwardMessageId.Should().NotBeEmpty();
            forwardMessageId.Should().Be(message.ForwardMessageId.Value);
        }

        [Fact]
        public void DeleteMessageAsync_ShouldThrowIfMessageDoesNotExist()
        {
            // Arrange
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Message)null)
                .Verifiable();

            var request = new DeleteMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"));

            // Act
            Func<Task> act = async () => { await _messageService.DeleteMessageAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to delete message. Message {nameof(request.MessageId)}:{request.MessageId} not found.");

            VerifyMocks();
        }

        [Fact]
        public void DeleteMessageAsync_ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Message())
                .Verifiable();
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            var request = new DeleteMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"));

            // Act
            Func<Task> act = async () => { await _messageService.DeleteMessageAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to delete message. Member {nameof(request.SaasUserId)}:{request.SaasUserId} not found.");

            VerifyMocks();
        }

        [Fact]
        public void DeleteMessageAsync_ShouldThrowIfMemberIsNotMessageOwner()
        {
            // Arrange
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Message { OwnerId = new Guid("F19AD922-B0DB-4686-8CB4-F51902800CAE") })
                .Verifiable();
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new Member { Id = new Guid("ABF2CA08-5374-4CED-BE87-6EA93A8B90DA") })
                .Verifiable();

            var request = new DeleteMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"));

            // Act
            Func<Task> act = async () => { await _messageService.DeleteMessageAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatAccessForbiddenException>()
                .And.Message.Should().Be($"Unable to delete message. Message {nameof(request.MessageId)}:{request.MessageId} owner required.");

            VerifyMocks();
        }

        [Fact]
        public async Task DeleteMessageAsync_ShouldDeleteDefaultMessageWithAttachmentsAndWithPreviousMessage()
        {
            // Arrange
            var request = new DeleteMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"));

            var messageOwnerId = new Guid("F19AD922-B0DB-4686-8CB4-F51902800CAE");
            var message = new Message
            {
                Id = request.MessageId,
                OwnerId = messageOwnerId,
                Type = MessageType.Default
            };
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.Is<Guid>(messageId => messageId.Equals(request.MessageId))))
                .ReturnsAsync(message)
                .Verifiable();

            var previousMessage = new Message
            {
                Id = Guid.NewGuid(),
                OwnerId = messageOwnerId,
                Type = MessageType.Default
            };
            _messageRepositoryMock.Setup(x => x.GetPreviousMessageAsync(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<DateTimeOffset>()))
                .ReturnsAsync(previousMessage)
                .Verifiable();
            _messageRepositoryMock.Setup(x => x.DeleteMessageAsync(It.Is<Guid>(messageId => messageId.Equals(request.MessageId))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasUserId => saasUserId.Equals(request.SaasUserId))))
                .ReturnsAsync(new Member { Id = messageOwnerId })
                .Verifiable();

            var attachments = new List<Attachment>
            {
                new Attachment(),
                new Attachment()
            };
            _attachmentRepositoryMock.Setup(x => x.GetMessageAttachmentsAsync(It.Is<Guid>(messageId => messageId.Equals(request.MessageId))))
                .ReturnsAsync(attachments)
                .Verifiable();
            _attachmentRepositoryMock.Setup(x => x.DeleteMessageAttachmentsAsync(It.Is<Guid>(messageId => messageId.Equals(request.MessageId))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _cloudAttachmentProviderMock.Setup(x => x.DeleteMessageAttachmentAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _channelMemberRepositoryMock.Setup(x => x.UpdateLastReadMessageAsync(
                    It.Is<Guid>(old => old.Equals(message.Id)),
                    It.Is<Guid?>(current => current.Equals(previousMessage.Id))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _messageService.DeleteMessageAsync(request);

            // Assert
            VerifyMocks();
            _cloudAttachmentProviderMock.Verify(prov => prov.DeleteMessageAttachmentAsync(It.IsAny<string>()), Times.Exactly(attachments.Count));
        }

        [Fact]
        public async Task DeleteMessageAsync_ShouldDeleteForwardMessageWithoutAttachmentsAndWithoutPreviousMessage()
        {
            // Arrange
            var request = new DeleteMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"));

            var messageOwnerId = new Guid("F19AD922-B0DB-4686-8CB4-F51902800CAE");
            var message = new Message
            {
                Id = request.MessageId,
                OwnerId = messageOwnerId,
                Type = MessageType.Forward,
                ForwardMessageId = new Guid("C7EFF82F-56FF-4966-9040-E79C90F118A7")
            };

            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.Is<Guid>(messageId => messageId.Equals(request.MessageId))))
                .ReturnsAsync(message)
                .Verifiable();
            _messageRepositoryMock.Setup(x => x.GetPreviousMessageAsync(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<DateTimeOffset>()))
                .ReturnsAsync((Message)null)
                .Verifiable();
            _messageRepositoryMock.Setup(x => x.DeleteMessageAsync(It.Is<Guid>(messageId => messageId.Equals(request.MessageId))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasUserId => saasUserId.Equals(request.SaasUserId))))
                .ReturnsAsync(new Member { Id = messageOwnerId })
                .Verifiable();

            _forwardMessageRepositoryMock.Setup(x => x.DeleteForwardMessageAsync(It.Is<Guid>(forwardMessageId => forwardMessageId.Equals(message.ForwardMessageId))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _attachmentRepositoryMock.Setup(x => x.GetMessageAttachmentsAsync(It.Is<Guid>(messageId => messageId.Equals(request.MessageId))))
                .ReturnsAsync(new List<Attachment>())
                .Verifiable();
            _attachmentRepositoryMock.Setup(x => x.DeleteMessageAttachmentsAsync(It.Is<Guid>(messageId => messageId.Equals(request.MessageId))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _channelMemberRepositoryMock.Setup(x => x.UpdateLastReadMessageAsync(
                    It.Is<Guid>(old => old.Equals(message.Id)),
                    It.Is<Guid?>(current => current.Equals(null))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _messageService.DeleteMessageAsync(request);

            // Assert
            VerifyMocks();
        }

        [Fact]
        public void UpdateMessageAsync_ShouldThrowIfMessageDoesNotExist()
        {
            // Arrange
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Message)null)
                .Verifiable();

            var request = new UpdateMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), "body");

            // Act
            Func<Task> act = async () => { await _messageService.UpdateMessageAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to update message. Message {nameof(request.MessageId)}:{request.MessageId} not found.");

            VerifyMocks();
        }

        [Fact]
        public void UpdateMessageAsync_ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Message())
                .Verifiable();
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            var request = new UpdateMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), "body");

            // Act
            Func<Task> act = async () => { await _messageService.UpdateMessageAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to update message. Member {nameof(request.SaasUserId)}:{request.SaasUserId} not found.");

            VerifyMocks();
        }

        [Fact]
        public void UpdateMessageAsync_ShouldThrowIfMemberIsNotMessageOwner()
        {
            // Arrange
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Message { OwnerId = new Guid("F19AD922-B0DB-4686-8CB4-F51902800CAE") })
                .Verifiable();
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new Member { Id = new Guid("ABF2CA08-5374-4CED-BE87-6EA93A8B90DA") })
                .Verifiable();

            var request = new UpdateMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), "body");

            // Act
            Func<Task> act = async () => { await _messageService.UpdateMessageAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatAccessForbiddenException>()
                .And.Message.Should().Be($"Unable to update message. Message {nameof(request.MessageId)}:{request.MessageId} owner required.");

            VerifyMocks();
        }

        [Fact]
        public async Task UpdateMessageAsync_ShouldUpdateMessageBodyAndUpdatedDate()
        {
            // Arrange
            var request = new UpdateMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), "new body");

            var member = new Member
            {
                Id = new Guid("ABF2CA08-5374-4CED-BE87-6EA93A8B90DA"),
                PhotoName = "photo name"
            };
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasUserId => saasUserId.Equals(request.SaasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            var message = new Message
            {
                Id = request.MessageId,
                OwnerId = member.Id,
                Owner = member,
                Body = "old bobby"
            };
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.Is<Guid>(messageId => messageId.Equals(request.MessageId))))
                .ReturnsAsync(message)
                .Verifiable();
            _messageRepositoryMock.Setup(x => x.UpdateMessageBodyAsync(
                    It.Is<Guid>(messageId => messageId.Equals(request.MessageId)),
                    It.Is<string>(body => body.Equals(request.Body)),
                    It.IsAny<DateTimeOffset>()
                    ))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _domainModelsMapperMock.Setup(x => x.MapToMessageResponse(It.IsAny<Message>(), It.IsAny<DateTimeOffset?>()))
                .Returns(new MessageResponse())
                .Verifiable();
            
            _dateTimeProviderMock.Setup(x => x.GetUtcNow())
                .Returns(DateTimeOffset.UtcNow)
                .Verifiable();

            // Act
            await _messageService.UpdateMessageAsync(request);

            // Assert
            VerifyMocks();
        }

        [Fact]
        public void GetMessageByIdAsync_ShouldThrowIfMessageDoesNotExist()
        {
            // Arrange
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Message)null)
                .Verifiable();

            var messageId = new Guid("D7CECB2A-076C-41E3-AB4B-9C0D49292CBA");

            // Act
            Func<Task> act = async () => { await _messageService.GetMessageByIdAsync(messageId); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to get message by {nameof(messageId)}. Message {nameof(messageId)}:{messageId} not found.");

            VerifyMocks();
        }

        [Fact]
        public async Task GetMessageByIdAsync_ShouldReturnMessage()
        {
            // Arrange
            var member = new Member
            {
                Id = new Guid("0B1837ED-8E84-450E-9BF1-DFC4AB766DFF"),
                PhotoName = "photo name"
            };
            var message = new Message
            {
                Id = new Guid("7C1F8365-715D-4135-ADD7-B033760947BF"),
                OwnerId = member.Id,
                Owner = member
            };

            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.Is<Guid>(messageId => messageId.Equals(message.Id))))
                .ReturnsAsync(message)
                .Verifiable();

            var messageResponse = new MessageResponse();
            _domainModelsMapperMock.Setup(x => x.MapToMessageResponse(It.IsAny<Message>(), It.IsAny<DateTimeOffset?>()))
                .Returns(messageResponse)
                .Verifiable();

            // Act
            var response = await _messageService.GetMessageByIdAsync(message.Id);

            // TODO: use automapper instead static mappings. Then check mapping calls and returned value
            // TODO: Get rid of static DateTime calls. Then check in tests.
            // Assert
            VerifyMocks();
        }

        private void VerifyMocks()
        {
            _dateTimeProviderMock.VerifyAll();

            _domainModelsMapperMock.VerifyAll();

            _channelRepositoryMock.VerifyAll();
            _memberRepositoryMock.VerifyAll();
            _messageRepositoryMock.VerifyAll();
            _forwardMessageRepositoryMock.VerifyAll();
            _channelMemberRepositoryMock.VerifyAll();
            _attachmentRepositoryMock.VerifyAll();

            _cloudImageProviderMock.VerifyAll();
            _cloudAttachmentProviderMock.VerifyAll();
        }
    }
}