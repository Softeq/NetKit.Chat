// Developed by Softeq Development Corporation
// http://www.softeq.com

using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20180905111500, "Rename member field")]
    public class RenameMemberField : Migration
    {
        protected override void Up()
        {
            Execute(@"sp_rename '[dbo].[Members].PhoneName', 'PhotoName', 'COLUMN';");
        }

        protected override void Down()
        {
            Execute(@"sp_rename '[dbo].[Members].PhotoName', 'PhoneName', 'COLUMN';");
        }
    }
}