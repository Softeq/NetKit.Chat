// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.Base
{
    public interface IBaseEntity<T>: IEntity
    {
        T Id { get; set; }
    }
}