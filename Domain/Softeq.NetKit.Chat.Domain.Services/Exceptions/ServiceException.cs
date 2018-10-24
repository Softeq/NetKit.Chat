// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Softeq.NetKit.Chat.Domain.Services.Exceptions.ErrorHandling;

namespace Softeq.NetKit.Chat.Domain.Services.Exceptions
{
    [Serializable]
    public class ServiceException : Exception, IServiceException
    {
        public List<ErrorDto> Errors { get; set; } = new List<ErrorDto>();

        public ServiceException(string message) : base(message)
        {
            InitializeErorrs(message);
        }

        public ServiceException(params ErrorDto[] errors)
        {
            InitializeErorrs(errors);
        }

        public ServiceException(string message, params ErrorDto[] errors) : base(message)
        {
            InitializeErorrs(errors);
        }

        public ServiceException(Exception innerException, params ErrorDto[] errors) : base("See inner exception.", innerException)
        {
            InitializeErorrs(errors);
        }

        protected ServiceException(string message, Exception innerException, params ErrorDto[] errors) : base(message, innerException)
        {
            InitializeErorrs(errors);
        }

        protected ServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        protected void InitializeErorrs(IEnumerable<ErrorDto> errors)
        {
            Errors.AddRange(errors);
        }
        protected void InitializeErorrs(string message)
        {
            Errors.Add(new ErrorDto(ErrorCode.UnknownError, message));
        }
    }
}