// Developed by Softeq Development Corporation
// http://www.softeq.com

using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20190501000000, "Add DirectChannelId And MessageType In Messages Table")]
    public class ModifyMessageTable : Migration
    {
        protected override void Up()
        {
            Execute(@"
                     ALTER TABLE dbo.Messages DROP CONSTRAINT FK_Messages_Channels_ChannelId

                     ALTER TABLE [dbo].[Messages] ALTER COLUMN ChannelId uniqueidentifier NULL                    
                    ");
        }

        protected override void Down()
        {
            Execute(@"
                     ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [FK_Messages_Channels_ChannelId] FOREIGN KEY([ChannelId])
                     REFERENCES [dbo].[Channels] ([Id])
                     
                     ALTER TABLE [dbo].[Messages] ALTER COLUMN ChannelId uniqueidentifier NOT NULL                     
                    ");
        }
    }
}
