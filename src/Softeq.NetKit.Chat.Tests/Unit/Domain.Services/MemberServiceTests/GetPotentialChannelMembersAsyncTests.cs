// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;
using Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Request.Member;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberServiceTests
{
    public class GetPotentialChannelMembersAsyncTests : MemberServiceTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldReturnPagedMembersResponse()
        {
            // Arrange
            var channelId = new Guid("8849DA19-C0E2-4C25-BDBA-2C14D7D304C8");

            var pageNumber = 1;
            var pageSize = 1;
            var nameFilter = "testFilter";

            var request = new GetPotentialChannelMembersRequest
            {
                NameFilter = nameFilter,
                PageSize = pageSize,
                PageNumber = pageNumber
            };

            var members = new QueryResult<Member>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Results = new List<Member>(),
                TotalNumberOfItems = 1,
                TotalNumberOfPages = 1
            };

            _memberRepositoryMock.Setup(x => x.GetPotentialChannelMembersAsync(
                    It.Is<Guid>(c => c.Equals(channelId)),
                    It.Is<int>(pn => pn.Equals(request.PageNumber)),
                    It.Is<int>(ps => ps.Equals(request.PageSize)),
                    It.Is<string>(nf => nf.Equals(request.NameFilter))))
                .ReturnsAsync(members)
                .Verifiable();

            // Act
            var result = await _memberService.GetPotentialChannelMembersAsync(channelId, request);

            result.Should().BeEquivalentTo(members);
        }
    }
}
