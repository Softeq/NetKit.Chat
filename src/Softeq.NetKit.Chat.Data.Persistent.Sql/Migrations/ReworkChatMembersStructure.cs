using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20190606111500, "Rename chat members")]
    public class ReworkChatMembersStructure : Migration
    {
        protected override void Up()
        {
            Execute(@"CREATE PROCEDURE dbo.GetChannelsLastMessage
                          AS
                          BEGIN

	                      SELECT Id, Body, Created, ImageUrl, Type, m.ChannelId, OwnerId, Updated, ForwardMessageId, AccessibilityStatus, ChannelType
	                      FROM Messages m
	                      INNER JOIN (
		                      SELECT ChannelId, MAX(Created) AS MaxDate 
                              FROM Messages
                              WHERE Messages.AccessibilityStatus = 0
		                      GROUP BY ChannelId
	                      ) mm 
                          ON m.ChannelId = mm.ChannelId and m.Created = mm.MaxDate
                          END");

            Execute(@"ALTER TABLE [dbo].[ChannelMembers] ADD Role INT NULL");
            Execute(@"UPDATE [dbo].[ChannelMembers] SET Role = 1 WHERE Role IS NULL");
            Execute(@"ALTER TABLE [dbo].[ChannelMembers] ALTER COLUMN Role INT NOT NULL");
        }

        protected override void Down()
        {
            Execute(@"DROP PROCEDURE dbo.GetChannelsLastMessage");
            Execute(@"ALTER TABLE [dbo].[ChannelMembers] DROP COLUMN Role");
        }
    }
}
