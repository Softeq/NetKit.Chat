﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.SignalR.Exceptions
{
    public class CommandNotFoundException : Exception
    {
        public CommandNotFoundException() { }

        public CommandNotFoundException(string message) : base(message) { }
    }
}