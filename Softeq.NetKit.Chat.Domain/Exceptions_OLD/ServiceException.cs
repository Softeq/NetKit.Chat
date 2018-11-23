// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Softeq.NetKit.Chat.Domain.Exceptions_OLD.ErrorHandling;

namespace Softeq.NetKit.Chat.Domain.Exceptions_OLD
{
    [Serializable]
    public class ServiceException : Exception
    {
        public List<ErrorDto> Errors { get; set; } = new List<ErrorDto>();

        public ServiceException(string message) : base(message)
        {
            InitializeErrors(message);
        }

        public ServiceException(params ErrorDto[] errors)
        {
            InitializeErrors(errors);
        }

        public ServiceException(string message, params ErrorDto[] errors) : base(message)
        {
            InitializeErrors(errors);
        }

        public ServiceException(Exception innerException, params ErrorDto[] errors) : base("See inner exception.", innerException)
        {
            InitializeErrors(errors);
        }

        protected ServiceException(string message, Exception innerException, params ErrorDto[] errors) : base(message, innerException)
        {
            InitializeErrors(errors);
        }

        protected ServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        protected void InitializeErrors(IEnumerable<ErrorDto> errors)
        {
            Errors.AddRange(errors);
        }

        protected void InitializeErrors(string message)
        {
            Errors.Add(new ErrorDto(ErrorCode.UnknownError, message));
        }
    }
}