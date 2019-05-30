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
                DECLARE @constraint NVARCHAR(100)
                SELECT @constraint = [name] FROM sys.objects WHERE name LIKE 'DF__Members__IsAfk%'
                IF (@constraint IS NOT NULL)
                BEGIN
                    EXEC ('ALTER TABLE [dbo].[Members] DROP CONSTRAINT ['+ @constraint +']')
                END

                ALTER TABLE [dbo].[Members] DROP COLUMN [IsAfk]
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
