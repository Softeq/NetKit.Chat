using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Repositories.Migrations
{
    [Migration(20181115111500, "Rename client class")]
    public class RenameClientModel : Migration
    {
        protected override void Up()
        {
            Execute(@"sp_rename '[dbo].[Clients]', 'Connections'");
        }

        protected override void Down()
        {
            Execute(@"sp_rename '[dbo].[Connections]', 'Clients'");
        }
    }
}
