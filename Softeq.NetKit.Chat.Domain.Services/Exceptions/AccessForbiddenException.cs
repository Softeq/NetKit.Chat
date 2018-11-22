// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Runtime.Serialization;
using Softeq.NetKit.Chat.Domain.Services.Exceptions.ErrorHandling;

namespace Softeq.NetKit.Chat.Domain.Services.Exceptions
{
    [Serializable]
    public class AccessForbiddenException : ServiceException
    {
        public AccessForbiddenException(params ErrorDto[] errors)
        {
            InitializeErrors(errors);
        }

        public AccessForbiddenException(string message) : base(message, new ErrorDto(ErrorCode.NotFound, message))
        {
        }

        public AccessForbiddenException(Exception innerException) : base("See inner exception.", innerException, new ErrorDto(ErrorCode.NotFound, innerException.Message))
        {
        }

        public AccessForbiddenException(string message, Exception innerException) : base(message, innerException, new ErrorDto(ErrorCode.NotFound, message))
        {
        }

        protected AccessForbiddenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}