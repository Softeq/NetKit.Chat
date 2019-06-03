// Developed by Softeq Development Corporation
// http://www.softeq.com

using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20190105100500, "Add DirectChannel table")]
    public class DirectChannel : Migration
    {
        protected override void Up()
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
            ");
        }

        protected override void Down()
        {
            Execute(@"
                ALTER TABLE dbo.DirectChannel DROP CONSTRAINT FK_DirectChannel_Members_OwnerId
                ALTER TABLE dbo.DirectChannel DROP CONSTRAINT FK_DirectChannel_Members_MemberId

                DROP TABLE dbo.DirectChannel
            ");
        }
    }
}
