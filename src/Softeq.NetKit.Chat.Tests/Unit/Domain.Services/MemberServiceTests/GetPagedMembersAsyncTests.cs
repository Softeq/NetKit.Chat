// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberService;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberServiceTests
{
    public class GetPagedMembersAsyncTests : MemberServiceTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldReturnPagedMembersResponse()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 1;
            var nameFilter = "testFilter";
            var saasUserId = "saasUserId";

            var members = new QueryResult<Member>()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Results = new List<Member>(),
                TotalNumberOfItems = 1,
                TotalNumberOfPages = 1
            };

            _memberRepositoryMock.Setup(x => x.GetPagedMembersExceptCurrentAsync(
                    It.Is<int>(pn => pn.Equals(pageNumber)),
                    It.Is<int>(ps => ps.Equals(pageSize)),
                    It.Is<string>(nf => nf.Equals(nameFilter)), 
                    It.IsAny<string>()))
                .ReturnsAsync(members)
                .Verifiable();

            // Act
            var act = await _memberService.GetPagedMembersAsync(pageNumber, pageSize, nameFilter, saasUserId);

            // Assert
            act.Should().BeEquivalentTo(members);
        }
    }
}
