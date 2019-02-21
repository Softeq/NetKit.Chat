// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators
{
    public class BaseRequestValidator<T> : AbstractValidator<T> where T : BaseRequest
    {
        public BaseRequestValidator()
        {
            RuleFor(x => x.RequestId).NotNull().NotEmpty();
        }
    }
}