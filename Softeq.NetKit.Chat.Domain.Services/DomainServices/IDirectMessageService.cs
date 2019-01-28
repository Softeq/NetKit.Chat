// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectMembers;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMembers;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    public interface IDirectMessageService
    {
        Task<DirectChannelResponse> CreateDirectMembers(CreateDirectMembersRequest createDirectMembersRequest);
        Task<DirectChannelResponse> GetDirectMembersById(Guid id);
    }
}
