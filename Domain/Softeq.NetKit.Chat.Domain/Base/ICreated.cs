// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Base
{
    public interface ICreated
    {
        DateTimeOffset Created { get; set; }
    }
}