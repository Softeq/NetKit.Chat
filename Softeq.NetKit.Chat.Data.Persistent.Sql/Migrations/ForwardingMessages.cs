// Developed by Softeq Development Corporation
// http://www.softeq.com

using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20181123100500, "ForwardMessages table")]
    public class ForwardingMessages : Migration
    {
        protected override void Up()
        {
            Execute(@"
                 CREATE TABLE dbo.ForwardMessages(
	                    Id        uniqueidentifier  NOT NULL PRIMARY KEY CLUSTERED,
	                    Body      nvarchar(max)     NULL,
	                    Created   datetimeoffset(7) NOT NULL,
	                    ChannelId uniqueidentifier  NOT NULL,
	                    OwnerId   uniqueidentifier  NULL
                );

                ALTER TABLE dbo.ForwardMessages 
                WITH CHECK ADD CONSTRAINT FK_ForwardMessages_Channels FOREIGN KEY(ChannelId)
                REFERENCES dbo.Channels (Id)

                ALTER TABLE dbo.ForwardMessages
                WITH CHECK ADD CONSTRAINT FK_ForwardMessages_Members FOREIGN KEY(OwnerId)
                REFERENCES dbo.Members (Id)

                ALTER TABLE dbo.Messages
                ADD ForwardMessageId uniqueidentifier NULL
            ");
        }

        protected override void Down()
        {
            Execute(@"
                ALTER TABLE [dbo].[Messages] DROP COLUMN ForwardMessageId
                DROP TABLE [dbo].[ForwardMessages]");
        }
    }
}
