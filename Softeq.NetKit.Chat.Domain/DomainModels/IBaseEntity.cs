// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.DomainModels
{
    public interface IBaseEntity<T> : IEntity
    {
        T Id { get; set; }
    }
}