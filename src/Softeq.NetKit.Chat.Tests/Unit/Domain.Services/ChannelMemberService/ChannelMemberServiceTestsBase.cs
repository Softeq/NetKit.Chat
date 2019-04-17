// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Data.SqlClient;
using Moq;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Data.Persistent.Sql;
using Softeq.NetKit.Chat.Data.Persistent.Sql.Database;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.Services.Mappings;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.ChannelMemberService
{
    public abstract class ChannelMemberServiceTestsBase
    {
        protected readonly IChannelMemberService _channelMemberService;
        protected readonly Mock<IUnitOfWork> _unitOfWorkMock;

        protected readonly Mock<ISqlConnectionFactory> _sqlConnectionFactoryMock = new Mock<ISqlConnectionFactory>(MockBehavior.Strict);
        protected readonly Mock<IChannelRepository> _channelRepositoryMock = new Mock<IChannelRepository>(MockBehavior.Strict);
        protected readonly Mock<IChannelMemberRepository> _channelMemberRepositoryMock = new Mock<IChannelMemberRepository>(MockBehavior.Strict);
        protected readonly Mock<IDomainModelsMapper> _domainModelsMapperMock = new Mock<IDomainModelsMapper>(MockBehavior.Strict);

        protected ChannelMemberServiceTestsBase()
        {
            _sqlConnectionFactoryMock.Setup(x => x.CreateConnection()).Returns(It.IsAny<SqlConnection>());
            _unitOfWorkMock = new Mock<UnitOfWork>(_sqlConnectionFactoryMock.Object).As<IUnitOfWork>();
            _unitOfWorkMock.CallBase = true;

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
