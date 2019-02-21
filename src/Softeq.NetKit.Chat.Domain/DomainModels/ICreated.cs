// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.DomainModels
{
    public interface ICreated
    {
        DateTimeOffset Created { get; set; }
    }
}