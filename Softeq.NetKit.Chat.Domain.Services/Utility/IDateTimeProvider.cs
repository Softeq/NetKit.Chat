// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Services.Utility
{
    public interface IDateTimeProvider
    {
        DateTimeOffset GetUtcNow();
    }
}