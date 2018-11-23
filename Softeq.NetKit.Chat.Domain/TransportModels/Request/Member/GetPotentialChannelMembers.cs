namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Member
{
    public class GetPotentialChannelMembers
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string NameFilter { get; set; }
    }
}
