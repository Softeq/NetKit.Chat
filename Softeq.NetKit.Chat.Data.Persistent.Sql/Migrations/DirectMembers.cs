// Developed by Softeq Development Corporation
// http://www.softeq.com

using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20190121100500, "Add Direct Members table")]
    public class DirectMembers : Migration
    {
        protected override void Up()
        {
            Execute(@"
                CREATE TABLE [dbo].DirectMembers(
                	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY CLUSTERED,
                    [Member01Id] [uniqueidentifier] NOT NULL,
                    [Member02Id] [uniqueidentifier] NOT NULL)

				ALTER TABLE [dbo].[DirectMembers] WITH CHECK ADD CONSTRAINT [FK_DirectMembers_Members_Member01Id] FOREIGN KEY([Member01Id])
                REFERENCES [dbo].[Members] ([Id])

                ALTER TABLE [dbo].[DirectMembers] WITH CHECK ADD CONSTRAINT [FK_DirectMembers_Members_Member02Id] FOREIGN KEY([Member02Id])
                REFERENCES [dbo].[Members] ([Id])  
            ");
        }

        protected override void Down()
        {
            Execute(@"
                ALTER TABLE dbo.DirectMembers DROP CONSTRAINT FK_DirectMembers_Members_Member01Id
                ALTER TABLE dbo.DirectMembers DROP CONSTRAINT FK_DirectMembers_Members_Member02Id

                DROP TABLE dbo.DirectMembers
            ");
        }
    }
}
