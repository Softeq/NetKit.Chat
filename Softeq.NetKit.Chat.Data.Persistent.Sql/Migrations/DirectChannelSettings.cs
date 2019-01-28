// Developed by Softeq Development Corporation
// http://www.softeq.com

using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20190122200500, "Add DirectChannelSettings table")]
    public class DirectChannelSettings : Migration
    {
        protected override void Up()
        {
            Execute(@"
                CREATE TABLE [dbo].DirectChannelSettings(
                    [DirectChannelId] [uniqueidentifier] NOT NULL,
                    [MemberId] [uniqueidentifier] NOT NULL,
                    [IsMuted] [bit] NOT NULL,
                    [IsPinned] [bit] NOT NULL,
                    [IsDisabled] [bit] NOT NULL         

                CONSTRAINT PK_DirectChannelSettings PRIMARY KEY (DirectChannelId, MemberId))

                ALTER TABLE [dbo].[DirectChannelSettings] WITH CHECK ADD CONSTRAINT [FK_DirectChannelSettings_DirectChannel_DirectChannelId] FOREIGN KEY([DirectChannelId])
                REFERENCES [dbo].[DirectChannel] ([Id])
            ");
        }

        protected override void Down()
        {
            Execute(@"
                    ALTER TABLE dbo.DirectChannelSettings DROP CONSTRAINT FK_DirectChannelSettings_DirectChannel_DirectChannelId

                    DROP TABLE dbo.DirectChannelSettings
            ");
        }
    }
}
