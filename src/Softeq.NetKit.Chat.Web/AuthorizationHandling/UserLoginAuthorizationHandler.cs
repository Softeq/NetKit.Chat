// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using System.Linq;
using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.Web.AuthorizationHandling
{
    public class UserLoginAuthorizationHandler : IAuthorizationHandler
    {
        private readonly IMemberService _memberService;

        public UserLoginAuthorizationHandler(IMemberService componentContext)
        {
            _memberService = componentContext;
        }

        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            const string saasUserIdType = "sub";
            var saasUserIdClaim = context.User?.Claims.FirstOrDefault(x => x.Type == saasUserIdType);

            if (saasUserIdClaim == null)
            {
                return;
            }

            await _memberService.ActivateMemberAsync(saasUserIdClaim.Value);
            context.Succeed(new OperationAuthorizationRequirement());
        }
    }
}
