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
                    [OwnerId] [uniqueidentifier] NOT NULL,
                    [MemberId] [uniqueidentifier] NOT NULL)

				ALTER TABLE [dbo].[DirectMembers] WITH CHECK ADD CONSTRAINT [FK_DirectMembers_Members_FirstMemberId] FOREIGN KEY([OwnerId])
                REFERENCES [dbo].[Members] ([Id])

                ALTER TABLE [dbo].[DirectMembers] WITH CHECK ADD CONSTRAINT [FK_DirectMembers_Members_SecondMemberId] FOREIGN KEY([MemberId])
                REFERENCES [dbo].[Members] ([Id])  
            ");
        }

        protected override void Down()
        {
            Execute(@"
                ALTER TABLE dbo.DirectMembers DROP CONSTRAINT FK_DirectMembers_Members_FirstMemberId
                ALTER TABLE dbo.DirectMembers DROP CONSTRAINT FK_DirectMembers_Members_SecondMemberId

                DROP TABLE dbo.DirectMembers
            ");
        }
    }
}
