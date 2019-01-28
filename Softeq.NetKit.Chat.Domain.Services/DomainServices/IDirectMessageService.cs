// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectMembers;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMembers;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    public interface IDirectMessageService
    {
        Task<DirectChannelResponse> CreateDirectChannel(CreateDirectMembersRequest createDirectMembersRequest);
        Task<DirectChannelResponse> GetDirectChannelById(Guid id);
    }
}
