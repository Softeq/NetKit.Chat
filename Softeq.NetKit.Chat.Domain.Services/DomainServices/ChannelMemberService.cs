// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnsureThat;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Exceptions.ErrorHandling;
using Softeq.NetKit.Chat.Domain.Services.Mappers;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.ChannelMember;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.ChannelMember;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    internal class ChannelMemberService : BaseService, IChannelMemberService
    {
        public ChannelMemberService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IReadOnlyCollection<ChannelMemberResponse>> GetChannelMembersAsync(ChannelRequest request)
        {
            var channelMembers = await UnitOfWork.ChannelMemberRepository.GetChannelMembersAsync(request.ChannelId);
            return channelMembers.Select(x => x.ToChannelMemberResponse()).ToList().AsReadOnly();
        }

        public async Task<ChannelMemberResponse> GetChannelMemberAsync(GetChannelMemberRequest request)
        {
            var channelMember = await UnitOfWork.ChannelMemberRepository.GetChannelMemberAsync(request.MemberId, request.ChannelId);
            Ensure.That(channelMember).WithException(x => new ServiceException(new ErrorDto(ErrorCode.NotFound, "Channel member does not exist."))).IsNotNull();
            return channelMember.ToChannelMemberResponse();
        }
    }
}