// Developed by Softeq Development Corporation
// http://www.softeq.com

using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20181123164700, "Add IsPinned field")]
    public class PinChannelField : Migration
    {
        protected override void Up()
        {
            Execute(@"ALTER TABLE [dbo].[ChannelMembers] ADD IsPinned bit NOT NULL DEFAULT 0");
        }

        protected override void Down()
        {
            Execute(@"ALTER TABLE [dbo].[ChannelMembers] DROP COLUMN IsPinned");
        }
    }
}