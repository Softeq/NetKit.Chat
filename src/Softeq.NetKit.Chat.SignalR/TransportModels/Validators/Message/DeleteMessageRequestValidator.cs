// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Message;
using Softeq.NetKit.Chat.TransportModels.Models.SignalRModels;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Message
{
    public class DeleteMessageRequestValidator : BaseRequestValidator<SignalRRequest<DeleteMessageRequest>, DeleteMessageRequest>
    {
        public DeleteMessageRequestValidator()
        {
            RuleFor(x => x.Request).NotNull();
            RuleFor(x => x.Request.MessageId).NotEmpty();
        }
    }
}