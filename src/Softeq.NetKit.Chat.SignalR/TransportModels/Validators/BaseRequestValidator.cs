// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request;
using Softeq.NetKit.Chat.TransportModels.Models.SignalRModels;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators
{
    public class BaseRequestValidator<T, TB> : AbstractValidator<T> where T : SignalRRequest<TB> where TB : BaseRequest
    {
        public BaseRequestValidator()
        {
            RuleFor(x => x.RequestId).NotNull().NotEmpty();
        }
    }
}