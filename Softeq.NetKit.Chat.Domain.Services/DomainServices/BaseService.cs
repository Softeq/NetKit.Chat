// Developed by Softeq Development Corporation
// http://www.softeq.com

using EnsureThat;
using Softeq.NetKit.Chat.Data.Persistent;

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