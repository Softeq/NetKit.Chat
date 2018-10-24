// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnsureThat;
using Softeq.NetKit.Chat.Data.Interfaces.UnitOfWork;
using Softeq.NetKit.Chat.Domain.Channel.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.ChannelMember;
using Softeq.NetKit.Chat.Domain.ChannelMember.TransportModels;
using Softeq.NetKit.Chat.Domain.Services.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Exceptions.ErrorHandling;

namespace Softeq.NetKit.Chat.Domain.Services.ChannelMember
{
    internal class ChannelMemberService : BaseService, IChannelMemberService
    {
        public ChannelMemberService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        public async Task<IEnumerable<ChannelMemberResponse>> GetChannelMembersAsync(ChannelRequest request)
        {
            var channelMembers = await UnitOfWork.ChannelMemberRepository.GetChannelMembersAsync(request.ChannelId);
            return channelMembers.Select(x => x.ToChannelMemberResponse());
        }

        public async Task<ChannelMemberResponse> GetChannelMemberAsync(GetChannelMemberRequest request)
        {
            var channelMember = await UnitOfWork.ChannelMemberRepository.GetChannelMemberAsync(request.MemberId, request.ChannelId);
            Ensure.That(channelMember).WithException(x => new ServiceException(new ErrorDto(ErrorCode.NotFound, "Channel member does not exist."))).IsNotNull();
            return channelMember.ToChannelMemberResponse();
        }
    }
}