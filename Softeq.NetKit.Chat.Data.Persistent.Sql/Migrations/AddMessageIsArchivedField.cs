// Developed by Softeq Development Corporation
// http://www.softeq.com

using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20190401164700, "Add IsArchived field in messages table")]
    public class AddMessageIsArchivedField : Migration
    {
        protected override void Up()
        {
            Execute(@"ALTER TABLE [dbo].[Messages] ADD IsArchived bit NOT NULL DEFAULT 0");
        }

        protected override void Down()
        {
            Execute(@"ALTER TABLE [dbo].[Messages] DROP IsArchived");
        }
    }
}
