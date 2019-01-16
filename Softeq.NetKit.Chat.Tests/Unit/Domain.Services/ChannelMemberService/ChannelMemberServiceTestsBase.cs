// Developed by Softeq Development Corporation
// http://www.softeq.com

using Moq;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.Services.Mappings;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.ChannelMemberService
{
    public abstract class ChannelMemberServiceTestsBase
    {
        protected readonly IChannelMemberService _channelMemberService;

        protected readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        protected readonly Mock<IChannelRepository> _channelRepositoryMock = new Mock<IChannelRepository>(MockBehavior.Strict);
        protected readonly Mock<IChannelMemberRepository> _channelMemberRepositoryMock = new Mock<IChannelMemberRepository>(MockBehavior.Strict);
        protected readonly Mock<IDomainModelsMapper> _domainModelsMapperMock = new Mock<IDomainModelsMapper>(MockBehavior.Strict);

        protected ChannelMemberServiceTestsBase()
        {
            _unitOfWorkMock.Setup(x => x.ChannelRepository).Returns(_channelRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.ChannelMemberRepository).Returns(_channelMemberRepositoryMock.Object);

            _channelMemberService = new Chat.Domain.Services.DomainServices.ChannelMemberService(
                _unitOfWorkMock.Object,
                _domainModelsMapperMock.Object);
        }

        protected void VerifyMocks()
        {
            _channelRepositoryMock.VerifyAll();
            _channelMemberRepositoryMock.VerifyAll();
            _domainModelsMapperMock.VerifyAll();
        }
    }
}
