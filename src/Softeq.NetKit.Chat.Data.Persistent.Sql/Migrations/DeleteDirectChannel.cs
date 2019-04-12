// Developed by Softeq Development Corporation
// http://www.softeq.com

using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20190223100500, "Delete Direct table")]
    public class DeleteDirectChannel : Migration
    {
        protected override void Up()
        {
            Execute(@"
                ALTER TABLE dbo.DirectChannel DROP CONSTRAINT FK_DirectChannel_Members_OwnerId
                ALTER TABLE dbo.DirectChannel DROP CONSTRAINT FK_DirectChannel_Members_MemberId

                ALTER TABLE [dbo].[DirectMessages] DROP CONSTRAINT FK_DirectMessages_DirectChannel_DirectChannelId
                ALTER TABLE [dbo].[DirectChannelSettings] DROP CONSTRAINT FK_DirectChannelSettings_DirectChannel_DirectChannelId

                DROP TABLE dbo.DirectChannel
                DROP TABLE dbo.DirectChannelSettings
            ");
        }

        protected override void Down()
        {
            Execute(@"
                CREATE TABLE [dbo].DirectChannel(
                	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY CLUSTERED,
                    [OwnerId] [uniqueidentifier] NOT NULL,
                    [MemberId] [uniqueidentifier] NOT NULL)

				ALTER TABLE [dbo].[DirectChannel] WITH CHECK ADD CONSTRAINT [FK_DirectChannel_Members_OwnerId] FOREIGN KEY([OwnerId])
                REFERENCES [dbo].[Members] ([Id])

                ALTER TABLE [dbo].[DirectChannel] WITH CHECK ADD CONSTRAINT [FK_DirectChannel_Members_MemberId] FOREIGN KEY([MemberId])
                REFERENCES [dbo].[Members] ([Id])  
                
                CREATE TABLE [dbo].DirectChannelSettings(
                    [DirectChannelId] [uniqueidentifier] NOT NULL,
                    [MemberId] [uniqueidentifier] NOT NULL,
                    [IsMuted] [bit] NOT NULL,
                    [IsPinned] [bit] NOT NULL,
                    [IsDisabled] [bit] NOT NULL         

                CONSTRAINT PK_DirectChannelSettings PRIMARY KEY (DirectChannelId, MemberId))

                ALTER TABLE [dbo].[DirectChannelSettings] WITH CHECK ADD CONSTRAINT [FK_DirectChannelSettings_DirectChannel_DirectChannelId] FOREIGN KEY([DirectChannelId])
                REFERENCES [dbo].[DirectChannel] ([Id])
                
                ALTER TABLE [dbo].[DirectMessages] ADD DirectChannelId uniqueidentifier NULL
            ");
        }
    }
}
