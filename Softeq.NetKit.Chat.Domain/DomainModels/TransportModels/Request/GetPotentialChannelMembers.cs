﻿namespace Softeq.NetKit.Chat.Domain.Member.TransportModels.Request
{
    public class GetPotentialChannelMembers
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string NameFilter { get; set; }
    }
}
