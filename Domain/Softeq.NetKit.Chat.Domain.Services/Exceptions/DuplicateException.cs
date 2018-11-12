// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using System;
using System.Runtime.Serialization;
using Softeq.NetKit.Chat.Domain.Services.Exceptions.ErrorHandling;

namespace Softeq.NetKit.Chat.Domain.Services.Exceptions
{
    [Serializable]
    public class DuplicateException : ServiceException
    {
        public DuplicateException(params ErrorDto[] errors)
        {
            InitializeErorrs(errors);
        }

        public DuplicateException(string message) : base(message, new ErrorDto(ErrorCode.NotFound, message))
        {
        }

        public DuplicateException(Exception innerException) : base("See inner exception.", innerException,
            new ErrorDto(ErrorCode.NotFound, innerException.Message))
        {
        }

        public DuplicateException(string message, Exception innerException) : base(message, innerException,
            new ErrorDto(ErrorCode.NotFound, message))
        {
        }

        protected DuplicateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}