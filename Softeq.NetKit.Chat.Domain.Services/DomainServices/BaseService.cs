// Developed by Softeq Development Corporation
// http://www.softeq.com

using EnsureThat;
using Softeq.NetKit.Chat.Infrastructure.Storage.Sql;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    internal class BaseService
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected BaseService(IUnitOfWork unitOfWork)
        {
            Ensure.That(unitOfWork).IsNotNull();

            UnitOfWork = unitOfWork;
        }
    }
}