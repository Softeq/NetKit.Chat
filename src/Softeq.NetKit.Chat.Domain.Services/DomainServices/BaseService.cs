// Developed by Softeq Development Corporation
// http://www.softeq.com

using EnsureThat;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Domain.Services.Mappings;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    internal class BaseService
    {
        protected BaseService(IUnitOfWork unitOfWork, IDomainModelsMapper domainModelsMapper)
        {
            Ensure.That(unitOfWork).IsNotNull();
            Ensure.That(domainModelsMapper).IsNotNull();

            UnitOfWork = unitOfWork;
            DomainModelsMapper = domainModelsMapper;
        }

        protected IUnitOfWork UnitOfWork { get; }

        protected IDomainModelsMapper DomainModelsMapper { get; }
    }
}