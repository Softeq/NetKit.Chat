// Developed by Softeq Development Corporation
// http://www.softeq.com

using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20190522113000, "Add database indexes")]
    public class AddDbIndexes : Migration
    {
        protected override void Up()
        {
            Execute(@"
                ALTER TABLE [Attachments]
                ALTER COLUMN [ContentType] NVARCHAR(100) NULL
                
                ALTER TABLE [Attachments]
                ALTER COLUMN [FileName] NVARCHAR(250) NULL
                
                ALTER TABLE [Clients]
                ALTER COLUMN [ClientConnectionId] NVARCHAR(100) NULL
                
                ALTER TABLE [Clients]
                ALTER COLUMN [Name] NVARCHAR(250) NULL
                
                ALTER TABLE [Clients]
                ALTER COLUMN [UserAgent] NVARCHAR(500) NULL
                
                ALTER TABLE [ForwardMessages]
                ALTER COLUMN [Body] NVARCHAR(1000) NULL
                
                ALTER TABLE [Members]
                ALTER COLUMN [Email] NVARCHAR(250) NULL
                
                ALTER TABLE [Members]
                ALTER COLUMN [Name] NVARCHAR(250) NULL
                
                ALTER TABLE [Members]
                ALTER COLUMN [PhotoName] NVARCHAR(250) NULL
                
                ALTER TABLE [Members]
                ALTER COLUMN [SaasUserId] NVARCHAR(50) NULL
                
                ALTER TABLE [Messages]
                ALTER COLUMN [Body] NVARCHAR(1000) NULL 
                
                ALTER TABLE [Messages]
                ALTER COLUMN [ImageUrl] NVARCHAR(250) NULL
                
                CREATE NONCLUSTERED INDEX [IX_FK_Attachments_Messages_MessageId] ON [dbo].[Attachments]([MessageId] ASC)
                CREATE NONCLUSTERED INDEX [IX_FK_ChannelMembers_Channels_ChannelId] ON [dbo].[ChannelMembers]([ChannelId] ASC)
                CREATE NONCLUSTERED INDEX [IX_FK_ChannelMembers_Members_MemberId] ON [dbo].[ChannelMembers]([MemberId] ASC)
                CREATE NONCLUSTERED INDEX [IX_FK_ChannelMembers_Messages_LastReadMessageId] ON [dbo].[ChannelMembers]([LastReadMessageId] ASC)
                CREATE NONCLUSTERED INDEX [IX_FK_Channels_Members_CreatorId] ON [dbo].[Channels]([CreatorId] ASC)
                CREATE NONCLUSTERED INDEX [IX_FK_Clients_Members_MemberId] ON [dbo].[Clients]([MemberId] ASC)
                CREATE NONCLUSTERED INDEX [IX_FK_ForwardMessages_Channels_ChannelId] ON [dbo].[ForwardMessages]([ChannelId] ASC)
                CREATE NONCLUSTERED INDEX [IX_FK_ForwardMessages_Members_OwnerId] ON [dbo].[ForwardMessages]([OwnerId] ASC)
                CREATE NONCLUSTERED INDEX [IX_FK_Messages_Members_OwnerId] ON [dbo].[Messages]([OwnerId] ASC)
                CREATE NONCLUSTERED INDEX [IX_FK_Notifications_Channels_ChannelId] ON [dbo].[Notifications]([ChannelId] ASC)
                CREATE NONCLUSTERED INDEX [IX_FK_Notifications_Members_MemberId] ON [dbo].[Notifications]([MemberId] ASC)
                CREATE NONCLUSTERED INDEX [IX_FK_Notifications_Messages_MessageId] ON [dbo].[Notifications]([MessageId] ASC)
                CREATE NONCLUSTERED INDEX [IX_FK_NotificationSettings_Members_MemberId] ON [dbo].[NotificationSettings]([MemberId] ASC)
                CREATE NONCLUSTERED INDEX [IX_FK_Settings_Channel_ChannelId] ON [dbo].[Settings]([ChannelId] ASC)
                
                CREATE NONCLUSTERED INDEX [IX_ChannelMembers_ChannelId_MemberId] ON [dbo].[ChannelMembers]([ChannelId] ASC, [MemberId] ASC)
                CREATE NONCLUSTERED INDEX [IX_Messages_Created_ChannelId_AccessibilityStatus] ON [dbo].[Messages]([Created] ASC, [ChannelId] ASC, [AccessibilityStatus] ASC)
                CREATE NONCLUSTERED INDEX [IX_Clients_ClientConnectionId] ON [dbo].[Clients]([ClientConnectionId] ASC)
                CREATE NONCLUSTERED INDEX [IX_Members_SaasUserId] ON [dbo].[Members]([SaasUserId] ASC)
                CREATE NONCLUSTERED INDEX [IX_Members_Name] ON [dbo].[Members]([Name] ASC)");
        }

        protected override void Down()
        {
            Execute(@"
                DROP INDEX [IX_FK_Attachments_Messages_MessageId] ON [Attachments]
                DROP INDEX [IX_FK_ChannelMembers_Channels_ChannelId] ON [ChannelMembers]
                DROP INDEX [IX_FK_ChannelMembers_Members_MemberId] ON [ChannelMembers]
                DROP INDEX [IX_FK_ChannelMembers_Messages_LastReadMessageId] ON [ChannelMembers]
                DROP INDEX [IX_FK_Channels_Members_CreatorId] ON [Channels]
                DROP INDEX [IX_FK_Clients_Members_MemberId] ON [Clients]
                DROP INDEX [IX_FK_ForwardMessages_Channels_ChannelId] ON [ForwardMessages]
                DROP INDEX [IX_FK_ForwardMessages_Members_OwnerId] ON [ForwardMessages]
                DROP INDEX [IX_FK_Messages_Members_OwnerId] ON [Messages]
                DROP INDEX [IX_FK_Notifications_Channels_ChannelId] ON [Notifications]
                DROP INDEX [IX_FK_Notifications_Members_MemberId] ON [Notifications]
                DROP INDEX [IX_FK_Notifications_Messages_MessageId] ON [Notifications]
                DROP INDEX [IX_FK_NotificationSettings_Members_MemberId] ON [NotificationSettings]
                DROP INDEX [IX_FK_Settings_Channel_ChannelId] ON [Settings]
                
                DROP INDEX [IX_ChannelMembers_ChannelId_MemberId] ON [ChannelMembers]
                DROP INDEX [IX_Messages_Created_ChannelId_AccessibilityStatus] ON [Messages]
                DROP INDEX [IX_Clients_ClientConnectionId] ON [Clients]
                DROP INDEX [IX_Members_SaasUserId] ON [Members]
                DROP INDEX [IX_Members_Name] ON [Members]
                
                ALTER TABLE [Attachments]
                ALTER COLUMN [ContentType] NVARCHAR(MAX) NULL
                
                ALTER TABLE [Attachments]
                ALTER COLUMN [FileName] NVARCHAR(MAX) NULL
                
                ALTER TABLE [Clients]
                ALTER COLUMN [ClientConnectionId] NVARCHAR(MAX) NULL
                
                ALTER TABLE [Clients]
                ALTER COLUMN [Name] NVARCHAR(MAX) NULL
                
                ALTER TABLE [Clients]
                ALTER COLUMN [UserAgent] NVARCHAR(MAX) NULL
                
                ALTER TABLE [ForwardMessages]
                ALTER COLUMN [Body] NVARCHAR(MAX) NULL
                
                ALTER TABLE [Members]
                ALTER COLUMN [Email] NVARCHAR(MAX) NULL
                
                ALTER TABLE [Members]
                ALTER COLUMN [Name] NVARCHAR(MAX) NULL
                
                ALTER TABLE [Members]
                ALTER COLUMN [PhotoName] NVARCHAR(MAX) NULL
                
                ALTER TABLE [Members]
                ALTER COLUMN [SaasUserId] NVARCHAR(MAX) NULL
                
                ALTER TABLE [Messages]
                ALTER COLUMN [Body] NVARCHAR(MAX) NULL 
                
                ALTER TABLE [Messages]
                ALTER COLUMN [ImageUrl] NVARCHAR(MAX) NULL");
        }
    }
}