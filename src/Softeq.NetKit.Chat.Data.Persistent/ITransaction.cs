// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Softeq.NetKit.Chat.Data.Persistent
{
    public interface ITransaction
    {
        Task ExecuteTransactionAsync(Func<Task> tranCallbackAsync, TransactionOptions? transactionOptions = null);
    }
}
