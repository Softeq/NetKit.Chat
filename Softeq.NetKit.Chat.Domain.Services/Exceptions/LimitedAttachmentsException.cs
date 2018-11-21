// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Runtime.Serialization;
using Softeq.NetKit.Chat.Domain.Services.Exceptions.ErrorHandling;

namespace Softeq.NetKit.Chat.Domain.Services.Exceptions
{
    [Serializable]
    public class LimitedAttachmentsException : ServiceException
    {
        public LimitedAttachmentsException(params ErrorDto[] errors)
        {
            InitializeErrors(errors);
        }

        public LimitedAttachmentsException(string message) : base(message, new ErrorDto(ErrorCode.NotFound, message))
        {
        }

        public LimitedAttachmentsException(Exception innerException) : base("See inner exception.", innerException, new ErrorDto(ErrorCode.NotFound, innerException.Message))
        {
        }

        public LimitedAttachmentsException(string message, Exception innerException) : base(message, innerException, new ErrorDto(ErrorCode.NotFound, message))
        {
        }

        protected LimitedAttachmentsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}