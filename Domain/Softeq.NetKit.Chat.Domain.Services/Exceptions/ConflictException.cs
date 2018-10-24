// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Runtime.Serialization;
using Softeq.NetKit.Chat.Domain.Services.Exceptions.ErrorHandling;

namespace Softeq.NetKit.Chat.Domain.Services.Exceptions
{
    [Serializable]
    public class ConflictException : ServiceException
    {
        public ConflictException(params ErrorDto[] errors)
        {
            InitializeErorrs(errors);
        }

        public ConflictException(string message) : base(message, new ErrorDto(ErrorCode.NotFound, message))
        {
        }

        public ConflictException(Exception innerException) : base("See inner exception.", innerException, new ErrorDto(ErrorCode.NotFound, innerException.Message))
        {
        }

        public ConflictException(string message, Exception innerException) : base(message, innerException, new ErrorDto(ErrorCode.NotFound, message))
        {
        }

        protected ConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}