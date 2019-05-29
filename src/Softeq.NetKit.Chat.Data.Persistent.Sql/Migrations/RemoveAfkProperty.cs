// Developed by Softeq Development Corporation
// http://www.softeq.com

using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20190529111500, "Remove Afk property")]
    public class RemoveAfkProperty : Migration
    {
        protected override void Up()
        {
            Execute(@"
                declare @constraint nvarchar(100)
                select @constraint = [name] from sys.objects where type = 'D' and name like 'DF__Members__IsAfk%'
                if (@constraint is not null)
                begin
                    exec ('alter table [dbo].[Members] drop constraint [' + @constraint +']')
                end

                ALTER TABLE [dbo].[Members] DROP  COLUMN [IsAfk]
            ");
        }

        protected override void Down()
        {
            Execute(@"
                ALTER TABLE [dbo].[Members] ADD [IsAfk] [bit] NOT NULL DEFAULT(0)
            ");
        }
    }
}
