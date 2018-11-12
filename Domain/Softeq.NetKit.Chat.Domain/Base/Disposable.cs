// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Base
{
    public class Disposable : IDisposable
    {
        private bool isDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Disposable()
        {
            Dispose(false);
        }

        protected virtual void DisposeCore()
        {
        }

        private void Dispose(bool disposing)
        {
            if (!isDisposed && disposing)
                DisposeCore();

            isDisposed = true;
        }
    }
}