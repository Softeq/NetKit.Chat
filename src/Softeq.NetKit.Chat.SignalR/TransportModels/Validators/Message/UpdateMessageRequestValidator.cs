// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Message;
using Softeq.NetKit.Chat.TransportModels.Models.SignalRModels;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Message
{
    public class UpdateMessageRequestValidator : BaseRequestValidator<SignalRRequest<UpdateMessageRequest>, UpdateMessageRequest>
    {
        public UpdateMessageRequestValidator()
        {
            RuleFor(x => x.Request.MessageId).NotEmpty();
            RuleFor(x => x.Request.Body).NotNull().NotEmpty();
        }
    }
}