// Developed by Softeq Development Corporation
// http://www.softeq.com

using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20190510111500, "Add deleted member field")]
    public class AddIsDeletedMemberField : Migration
    {
        protected override void Up()
        {
            Execute(@"ALTER TABLE [dbo].[Members] ADD IsDeleted bit NOT NULL DEFAULT 0");
        }

        protected override void Down()
        {
            Execute(@"ALTER TABLE [dbo].[Members] DROP COLUMN IsDeleted");
        }
    }
}