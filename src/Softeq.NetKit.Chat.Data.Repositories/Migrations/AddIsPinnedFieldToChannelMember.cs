using System;
using System.Collections.Generic;
using System.Text;
using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Repositories.Migrations
{
    [Migration(20181121134700, "Add IsPinned field to channel member")]
    class AddIsPinnedFieldToChannelMember : Migration
    {
        protected override void Up()
        {
            Execute(@"ALTER TABLE [dbo].[ChannelMembers] ADD IsPinned bit DEFAULT 0 NOT NULL");
        }

        protected override void Down()
        {
            Execute(@"ALTER TABLE [dbo].[ChannelMembers] DROP COLUMN IsPinned");
        }
    }
}
