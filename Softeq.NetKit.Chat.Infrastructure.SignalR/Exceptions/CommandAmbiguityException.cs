// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;

namespace Softeq.NetKit.Chat.Infrastructure.SignalR.Exceptions
{
    public class CommandAmbiguityException : Exception
    {
        public IEnumerable<string> Ambiguities { get; set; }

        public CommandAmbiguityException(IEnumerable<string> ambiguities)
        {
            Ambiguities = ambiguities;
        }

        public CommandAmbiguityException(string message) : base(message) { }
    }
}