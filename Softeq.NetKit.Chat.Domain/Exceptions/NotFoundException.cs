// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Runtime.Serialization;
using Softeq.NetKit.Chat.Domain.Exceptions.ErrorHandling;

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    [Serializable]
    public class NotFoundException : ServiceException
    {
        public NotFoundException(params ErrorDto[] errors)
        {
            InitializeErrors(errors);
        }

        public NotFoundException(string message) : base(message, new ErrorDto(ErrorCode.NotFound, message))
        {
        }

        public NotFoundException(Exception innerException) : base("See inner exception.", innerException, new ErrorDto(ErrorCode.NotFound, innerException.Message))
        {
        }

        public NotFoundException(string message, Exception innerException) : base(message, innerException, new ErrorDto(ErrorCode.NotFound, message))
        {
        }

        protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}