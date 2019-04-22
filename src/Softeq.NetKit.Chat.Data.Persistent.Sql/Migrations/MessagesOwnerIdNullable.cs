// Developed by Softeq Development Corporation
// http://www.softeq.com

using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20190418171900, "Alter Messages OwnerId to be Nullable")]
    public class MessagesOwnerIdNullable : Migration
    {
        protected override void Up()
        {
            Execute(@"ALTER TABLE [dbo].[Messages] ALTER COLUMN OwnerId UNIQUEIDENTIFIER NULL");
        }

        protected override void Down()
        {
            Execute(@"ALTER TABLE [dbo].[Messages] ALTER COLUMN OwnerId UNIQUEIDENTIFIER NOT NULL");
        }
    }
}