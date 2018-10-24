// Developed by Softeq Development Corporation
// http://www.softeq.com

using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Repositories.Migrations
{
    [Migration(20180911164700, "Add сhannel photo field")]
    public class AddChannelPhotoField : Migration
    {
        protected override void Up()
        {
            Execute(@"ALTER TABLE [dbo].[Channels] ADD PhotoUrl nvarchar(500) NULL");
        }

        protected override void Down()
        {
            Execute(@"ALTER TABLE [dbo].[Channels] DROP COLUMN PhotoUrl");
        }
    }
}