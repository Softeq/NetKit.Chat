// Developed by Softeq Development Corporation
// http://www.softeq.com

using EnsureThat;
using Softeq.NetKit.Chat.Infrastructure.Storage.Sql;

namespace Softeq.NetKit.Chat.Domain.Services
{
    internal class BaseService
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected BaseService(IUnitOfWork unitOfWork)
        {
            Ensure.That(unitOfWork, "unitOfWork").IsNotNull();
            
            UnitOfWork = unitOfWork;
        }
    }
}