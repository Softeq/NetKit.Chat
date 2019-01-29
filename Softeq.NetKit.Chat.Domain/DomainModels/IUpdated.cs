// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.DomainModels
{
    public interface IUpdated
    {
        DateTimeOffset Updated { get; set; }
    }
}
