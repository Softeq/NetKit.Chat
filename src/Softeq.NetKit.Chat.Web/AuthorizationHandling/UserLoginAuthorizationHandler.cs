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

        public UserLoginAuthorizationHandler(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            const string claimType = "sub";
            var saasUserIdClaim = context.User?.Claims.FirstOrDefault(x => x.Type == claimType);

            if (saasUserIdClaim == null)
            {
                return;
            }

            await _memberService.ActivateMemberAsync(saasUserIdClaim.Value);
            context.Succeed(new OperationAuthorizationRequirement());
        }
    }
}
