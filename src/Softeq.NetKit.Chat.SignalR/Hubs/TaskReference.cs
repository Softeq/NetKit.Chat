// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.SignalR.Hubs
{
    public class TaskReference
    {
        private readonly Func<Task> _func;

        public TaskReference(Func<Task> func)
        {
            _func = func;
        }

        public async Task RunAsync()
        {
            await _func();
        }
    }

    public class TaskReference<T>
    {
        private readonly Func<Task<T>> _func;

        public TaskReference(Func<Task<T>> func)
        {
            _func = func;
        }

        public Task<T> RunAsync()
        {
            return _func();
        }
    }
}