using System.Collections.Generic;

namespace Softeq.NetKit.Chat.Domain.DomainModels
{
    public class QueryResult<T>
    {
        public IEnumerable<T> Entities { get; set; }
        public int TotalRows { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
