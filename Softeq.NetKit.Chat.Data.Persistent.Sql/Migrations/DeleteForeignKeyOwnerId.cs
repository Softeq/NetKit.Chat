// Developed by Softeq Development Corporation
// http://www.softeq.com

using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20190106154700, "Delete ForeignKey OwnerId")]
    public class DeleteForeignKeyOwnerId : Migration
    {
        protected override void Up()
        {
            Execute(@"ALTER TABLE dbo.Messages DROP CONSTRAINT FK_Messages_Members_OwnerId");
        }

        protected override void Down()
        {
            Execute(@"ALTER TABLE[dbo].[Messages] WITH CHECK ADD CONSTRAINT[FK_Messages_Members_OwnerId] FOREIGN KEY([OwnerId])
                    REFERENCES[dbo].[Members] ([Id])");
        }
    }
}


