﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20190122100500, "Add Direct messages table")]
    public class DirectMessages : Migration
    {
        protected override void Up()
        {
            Execute(@"
                CREATE TABLE [dbo].DirectMessages(
                	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY CLUSTERED,
                    [DirectMembersID] [uniqueidentifier] NOT NULL,
                    [Created] [datetimeoffset](7) NOT NULL,
                    [OwnerId] [uniqueidentifier] NOT NULL,
                    [Body] [nvarchar](MAX),
                    [Updated] [datetimeoffset](7) NOT NULL)

				ALTER TABLE [dbo].[DirectMessages] WITH CHECK ADD CONSTRAINT [FK_DirectMessages_DirectMembers_DirectMembersID] FOREIGN KEY([DirectMembersID])
                REFERENCES [dbo].[DirectMembers] ([Id])
            ");
        }

        protected override void Down()
        {
            Execute(@"
               ALTER TABLE dbo.DirectMessages DROP CONSTRAINT FK_DirectMessages_DirectMembers_DirectMembersID

               DROP TABLE dbo.DirectMembers
            ");
        }
    }
}
