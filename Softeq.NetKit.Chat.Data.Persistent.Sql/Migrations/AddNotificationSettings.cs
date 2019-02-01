using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20190102144700, "Add Notification Settings")]
    public class AddNotificationSettings : Migration
    {
        protected override void Up()
        {
            Execute(@"
                CREATE TABLE dbo.NotificationSettings(
	               Id                            uniqueidentifier  NOT NULL  PRIMARY KEY CLUSTERED,
	               MemberId                      uniqueidentifier  NOT NULL,
	               IsChannelNotificationsDisabled  bit               NOT NULL  DEFAULT 0
                );

                ALTER TABLE dbo.NotificationSettings
                WITH CHECK ADD CONSTRAINT FK_NotificationSettings_Members FOREIGN KEY(MemberId)
                REFERENCES dbo.Members (Id) ON DELETE CASCADE
            ");
        }

        protected override void Down()
        {
            Execute(@"
                ALTER TABLE dbo.NotificationSettings DROP CONSTRAINT FK_NotificationSettings_Members
                DROP TABLE dbo.NotificationSettings
            ");
        }
    }
}
