﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMembers
{
    public class DirectChannelResponse
    {
        public Guid DirectMembersId { get; set; }
        public MemberSummary Owner { get; set; }
        public MemberSummary Member { get; set; }
    }
}