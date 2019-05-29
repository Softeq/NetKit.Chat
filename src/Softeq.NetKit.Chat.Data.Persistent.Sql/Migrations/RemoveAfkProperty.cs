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
                select @constraint = [name] FROM sys.objects WHERE type = 'D' AND name LIKE 'DF__Members__IsAfk%'
                IF (@constraint is not null)
                BEGIN
                    EXEC ('ALTER TABLE [dbo].[Members] DROP CONSTRAINT [' + @constraint +']')
                END

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
