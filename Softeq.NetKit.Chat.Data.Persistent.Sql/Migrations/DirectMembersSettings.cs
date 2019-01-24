// Developed by Softeq Development Corporation
// http://www.softeq.com

using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20190122200500, "Add Direct Members Settings table")]
    public class DirectMembersSettings : Migration
    {
        protected override void Up()
        {
            Execute(@"
                CREATE TABLE [dbo].DirectMembersSettings(
                    [DirectMembersId] [uniqueidentifier] NOT NULL,
                    [MemberId] [uniqueidentifier] NOT NULL,
                    [IsMuted] [bit] NOT NULL,
                    [IsPinned] [bit] NOT NULL,
                    [IsDisabled] [bit] NOT NULL         

                CONSTRAINT PK_DirectMembersSettings PRIMARY KEY (DirectMembersId, MemberId))

                ALTER TABLE [dbo].[DirectMembersSettings] WITH CHECK ADD CONSTRAINT [FK_DirectMembersSettings_DirectMembers_DirectMemberId] FOREIGN KEY([DirectMembersId])
                REFERENCES [dbo].[DirectMembers] ([Id])
            ");
        }

        protected override void Down()
        {
            Execute(@"
                    ALTER TABLE dbo.DirectMembersSettings DROP CONSTRAINT FK_DirectMembersSettings_DirectMembers_DirectMemberId

                    DROP TABLE dbo.DirectMembersSettings
            ");
        }
    }
}
