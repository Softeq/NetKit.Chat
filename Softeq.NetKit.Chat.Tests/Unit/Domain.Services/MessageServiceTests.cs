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
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Message;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services
{
    public class MessageServiceTests
    {
        private readonly IMessageService _messageService;

        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IChannelRepository> _channelRepositoryMock = new Mock<IChannelRepository>();
        private readonly Mock<IMemberRepository> _memberRepositoryMock = new Mock<IMemberRepository>();
        private readonly Mock<IMessageRepository> _messageRepositoryMock = new Mock<IMessageRepository>();
        private readonly Mock<IForwardMessageRepository> _forwardMessageRepositoryMock = new Mock<IForwardMessageRepository>();
        private readonly Mock<IChannelMemberRepository> _channelMemberRepositoryMock = new Mock<IChannelMemberRepository>();
        private readonly Mock<IAttachmentRepository> _attachmentRepositoryMock = new Mock<IAttachmentRepository>();

        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly Mock<IConfigurationSection> _configurationSectionMock = new Mock<IConfigurationSection>();

        private readonly Mock<ICloudImageProvider> _cloudImageProviderMock = new Mock<ICloudImageProvider>();
        private readonly Mock<ICloudAttachmentProvider> _cloudAttachmentProviderMock = new Mock<ICloudAttachmentProvider>();

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

            _messageService = new MessageService(_unitOfWorkMock.Object, attachmentConfiguration, _cloudImageProviderMock.Object, _cloudAttachmentProviderMock.Object);
        }

        [Fact]
        public void CreateMessageAsync_ShouldThrowIfMemberDoesNotExists()
        {
            _channelRepositoryMock.Setup(x => x.IsChannelExistsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false)
                .Verifiable();

            var request = new CreateMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), MessageType.Default, "body");

            Func<Task> act = async () => { await _messageService.CreateMessageAsync(request); };

            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to create message. Channel {nameof(request.ChannelId)}:{request.ChannelId} not found.");

            _channelRepositoryMock.VerifyAll();
        }

        [Fact]
        public void CreateMessageAsync_ShouldThrowIfChannelDoesNotExists()
        {
            _channelRepositoryMock.Setup(x => x.IsChannelExistsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true)
                .Verifiable();
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            var request = new CreateMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), MessageType.Default, "body");

            Func<Task> act = async () => { await _messageService.CreateMessageAsync(request); };

            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to create message. Member {nameof(request.SaasUserId)}:{request.SaasUserId} not found.");

            _channelRepositoryMock.VerifyAll();
            _memberRepositoryMock.VerifyAll();
        }

        [Fact]
        public void CreateMessageAsync_ShouldThrowIfForwardMessageDoesNotExists()
        {
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

            Func<Task> act = async () => { await _messageService.CreateMessageAsync(request); };

            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to create message. Forward message {nameof(request.ForwardedMessageId)}:{request.ForwardedMessageId} not found.");

            _channelRepositoryMock.VerifyAll();
            _memberRepositoryMock.VerifyAll();
            _messageRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task CreateMessageAsync_ShouldCreateDefaultMessage()
        {
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

            Guid messageId = Guid.Empty;
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .Callback<Guid>(x => messageId = x)
                .ReturnsAsync((Message)null)
                .Verifiable();

            var messageResponse = await _messageService.CreateMessageAsync(request);

            _channelRepositoryMock.VerifyAll();
            _memberRepositoryMock.VerifyAll();
            _messageRepositoryMock.VerifyAll();
            _channelMemberRepositoryMock.VerifyAll();
            _forwardMessageRepositoryMock.VerifyAll();

            message.Id.Should().Be(messageId);
            message.ChannelId.Should().Be(request.ChannelId);
            message.OwnerId.Should().Be(member.Id);
            message.Body.Should().Be(request.Body);
            message.Type.Should().Be(request.Type);
            message.ImageUrl.Should().Be(request.ImageUrl);
        }

        [Fact]
        public async Task CreateMessageAsync_ShouldCreateForwardMessage()
        {
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

            Guid forwardMessageId = Guid.Empty;
            _forwardMessageRepositoryMock.Setup(x => x.AddForwardMessageAsync(It.IsAny<ForwardMessage>()))
                .Callback<ForwardMessage>(x => forwardMessageId = x.Id)
                .Returns(Task.CompletedTask)
                .Verifiable();

            var messageResponse = await _messageService.CreateMessageAsync(request);

            _channelRepositoryMock.VerifyAll();
            _memberRepositoryMock.VerifyAll();
            _messageRepositoryMock.VerifyAll();
            _channelMemberRepositoryMock.VerifyAll();
            _forwardMessageRepositoryMock.VerifyAll();
            _messageRepositoryMock.Verify(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()), Times.Exactly(2));

            forwardMessageId.Should().NotBeEmpty();
            forwardMessageId.Should().Be(message.ForwardMessageId.Value);
        }

        [Fact]
        public void DeleteMessageAsync_ShouldThrowIfMessageDoesNotExists()
        {
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Message)null)
                .Verifiable();

            var request = new DeleteMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"));

            Func<Task> act = async () => { await _messageService.DeleteMessageAsync(request); };

            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to delete message. Message {nameof(request.MessageId)}:{request.MessageId} not found.");

            _messageRepositoryMock.VerifyAll();
        }

        [Fact]
        public void DeleteMessageAsync_ShouldThrowIfMemberDoesNotExists()
        {
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Message())
                .Verifiable();
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            var request = new DeleteMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"));

            Func<Task> act = async () => { await _messageService.DeleteMessageAsync(request); };

            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to delete message. Member {nameof(request.SaasUserId)}:{request.SaasUserId} not found.");

            _messageRepositoryMock.VerifyAll();
            _memberRepositoryMock.VerifyAll();
        }

        [Fact]
        public void DeleteMessageAsync_ShouldThrowIfMemberIsNotMessageOwner()
        {
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Message { OwnerId = new Guid("F19AD922-B0DB-4686-8CB4-F51902800CAE") })
                .Verifiable();
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new Member { Id = new Guid("ABF2CA08-5374-4CED-BE87-6EA93A8B90DA") })
                .Verifiable();

            var request = new DeleteMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"));

            Func<Task> act = async () => { await _messageService.DeleteMessageAsync(request); };

            act.Should().Throw<NetKitChatAccessForbiddenException>()
                .And.Message.Should().Be($"Unable to delete message. Message {nameof(request.MessageId)}:{request.MessageId} owner required.");

            _messageRepositoryMock.VerifyAll();
            _memberRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task DeleteMessageAsync_ShouldDeleteMessageWithAttachmentsAndWithoutPreviousMessage()
        {
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
            _messageRepositoryMock.Setup(x => x.GetPreviousMessageAsync(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<DateTimeOffset>()))
                .ReturnsAsync((Message)null)
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

            await _messageService.DeleteMessageAsync(request);

            _messageRepositoryMock.VerifyAll();
            _memberRepositoryMock.VerifyAll();
            _attachmentRepositoryMock.VerifyAll();
            _cloudAttachmentProviderMock.VerifyAll();
            _cloudAttachmentProviderMock.Verify(prov => prov.DeleteMessageAttachmentAsync(It.IsAny<string>()), Times.Exactly(attachments.Count));
        }
    }
}