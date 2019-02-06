// Developed by Softeq Development Corporation
// http://www.softeq.com

using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20190401184700, "Add DirectChannelId And Channel Type In Messages Table")]
    public class AddDirectChannelIdAndChannelTypeInMessagesTable : Migration
    {
        protected override void Up()
        {
            Execute(@"
                      ALTER TABLE [dbo].[Messages] ADD DirectChannelId uniqueidentifier NULL
                      ALTER TABLE [dbo].[Messages] ADD ChannelType int NOT NULL DEFAULT 0
                    ");
        }

        protected override void Down()
        {
            Execute(@"
                     ALTER TABLE [dbo].[Messages] DROP DirectChannelId
                     ALTER TABLE [dbo].[Messages] DROP ChannelType
                    ");
        }
    }
}
