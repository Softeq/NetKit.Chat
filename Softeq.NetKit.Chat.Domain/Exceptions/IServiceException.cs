// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using Softeq.NetKit.Chat.Domain.Exceptions.ErrorHandling;

namespace Softeq.NetKit.Chat.Domain.Exceptions
{
    public interface IServiceException
    {
        List<ErrorDto> Errors { get; set; }
    }
}