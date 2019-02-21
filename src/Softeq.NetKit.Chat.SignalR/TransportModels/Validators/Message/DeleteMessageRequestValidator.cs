// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.Message;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Message
{
    public class DeleteMessageRequestValidator : BaseRequestValidator<DeleteMessageRequest>
    {
        public DeleteMessageRequestValidator()
        {
            RuleFor(x => x.MessageId).NotEmpty();
        }
    }
}