using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20181219154700, "Add IsActive user filed")]
    public class AddIsActiveUserFiled : Migration
    {
        protected override void Up()
        {
            Execute(@"ALTER TABLE [dbo].[Members] ADD IsActive bit NOT NULL DEFAULT 0");
        }

        protected override void Down()
        {
            Execute(@"ALTER TABLE [dbo].[ChannelMembers] DROP COLUMN IsActive");
        }
    }
}
