// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Repositories.Migrations
{
    [Migration(20180426191500, "Initial database")]
    public class InitialDatabase : Migration
    {
        protected override void Up()
        {
            Execute(@"
                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON
                SET ANSI_PADDING ON 
                
                -- Create Members table
                CREATE TABLE [dbo].[Members](
                	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
                	[Email] [nvarchar](max) NULL,
                	[IsAfk] [bit] NOT NULL DEFAULT(0),
                	[IsBanned] [bit] NOT NULL DEFAULT(0),
                	[LastActivity] [datetimeoffset](7) NOT NULL,
                	[LastNudged] [datetimeoffset](7) NULL,
                	[Name] [nvarchar](max) NULL,
                	[PhoneName] [nvarchar](max) NULL,
                	[Role] [int] NOT NULL DEFAULT(0),
                	[SaasUserId] [nvarchar](max) NULL,
                	[Status] [int] NOT NULL DEFAULT(0))
                
                CREATE UNIQUE NONCLUSTERED INDEX [IX_Id] ON [dbo].[Members] ([Id] ASC)
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                
                -- Create Clients table
                CREATE TABLE [dbo].[Clients](
                	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
                	[ClientConnectionId] [nvarchar](max) NULL,
                	[LastActivity] [datetimeoffset](7) NOT NULL,
                	[LastClientActivity] [datetimeoffset](7) NOT NULL,
                	[Name] [nvarchar](max) NULL,
                	[UserAgent] [nvarchar](max) NULL,
                	[MemberId] [uniqueidentifier] NOT NULL)

                ALTER TABLE [dbo].[Clients]  WITH CHECK ADD CONSTRAINT [FK_Clients_Members_MemberId] FOREIGN KEY([MemberId])
                REFERENCES [dbo].[Members] ([Id])
                
                ALTER TABLE [dbo].[Clients] CHECK CONSTRAINT [FK_Clients_Members_MemberId]
                
                -- Create Channels table
                CREATE TABLE [dbo].[Channels](
                	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
                	[IsClosed] [bit] NOT NULL DEFAULT(0),
                	[Created] [datetimeoffset](7) NOT NULL,
                	[CreatorId] [uniqueidentifier] NOT NULL,
                	[Name] [nvarchar](200) NULL,
                    [MembersCount] [int] NOT NULL DEFAULT(0),
                	[Type] [int] NOT NULL DEFAULT(0),
                	[Description] [nvarchar](80) NULL,
                	[WelcomeMessage] [nvarchar](200) NULL,
                	[Updated] [datetimeoffset](7) NULL)
                
                ALTER TABLE [dbo].[Channels]  WITH CHECK ADD CONSTRAINT [FK_Channels_Members_CreatorId] FOREIGN KEY([CreatorId])
                REFERENCES [dbo].[Members] ([Id])
                
                ALTER TABLE [dbo].[Channels] CHECK CONSTRAINT [FK_Channels_Members_CreatorId]
                
                -- Create Settings table
                CREATE TABLE [dbo].[Settings](
                	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
                	[RawSettings] [nvarchar](max) NULL,
                    [ChannelId] [uniqueidentifier] NOT NULL UNIQUE FOREIGN KEY REFERENCES [dbo].[Channels](Id))

                -- Create Messages table
                CREATE TABLE [dbo].[Messages](
                	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
                	[Body] [nvarchar](max) NULL,
                	[Created] [datetimeoffset](7) NOT NULL,
                	[ImageUrl] [nvarchar](max) NULL,
                	[Type] [int] NOT NULL DEFAULT(0),
                	[ChannelId] [uniqueidentifier] NOT NULL,
                	[OwnerId] [uniqueidentifier] NULL,
                	[Updated] [datetimeoffset](7) NULL)

                ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [FK_Messages_Channels_ChannelId] FOREIGN KEY([ChannelId])
                REFERENCES [dbo].[Channels] ([Id])
                
                ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [FK_Messages_Channels_ChannelId]
                
                ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [FK_Messages_Members_OwnerId] FOREIGN KEY([OwnerId])
                REFERENCES [dbo].[Members] ([Id])
                
                ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [FK_Messages_Members_OwnerId]
                
                -- Create Notifications table
                CREATE TABLE [dbo].[Notifications](
                	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
                	[IsRead] [bit] NOT NULL,
                	[MessageId] [uniqueidentifier] NOT NULL,
                	[ChannelId] [uniqueidentifier] NOT NULL,
                	[MemberId] [uniqueidentifier] NOT NULL)
                
                ALTER TABLE [dbo].[Notifications]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Notifications_dbo.Messages_MessageId] FOREIGN KEY([MessageId])
                REFERENCES [dbo].[Messages] ([Id])
                ON DELETE CASCADE
                
                ALTER TABLE [dbo].[Notifications] CHECK CONSTRAINT [FK_dbo.Notifications_dbo.Messages_MessageId]
                
                ALTER TABLE [dbo].[Notifications]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Notifications_dbo.Channels_ChannelId] FOREIGN KEY([ChannelId])
                REFERENCES [dbo].[Channels] ([Id])
                ON DELETE CASCADE
                
                ALTER TABLE [dbo].[Notifications] CHECK CONSTRAINT [FK_dbo.Notifications_dbo.Channels_ChannelId]
                
                ALTER TABLE [dbo].[Notifications]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Notifications_dbo.Members_MemberId] FOREIGN KEY([MemberId])
                REFERENCES [dbo].[Members] ([Id])
                ON DELETE CASCADE
                
                ALTER TABLE [dbo].[Notifications] CHECK CONSTRAINT [FK_dbo.Notifications_dbo.Members_MemberId]
                
                CREATE NONCLUSTERED INDEX [IX_MessageId] ON [dbo].[Notifications] ([MessageId] ASC)
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                
                CREATE NONCLUSTERED INDEX [IX_ChannelId] ON [dbo].[Notifications] ([ChannelId] ASC)
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                
                CREATE NONCLUSTERED INDEX [IX_MemberId] ON [dbo].[Notifications] ([MemberId] ASC)
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                
                -- Create ChannelMembers table
                CREATE TABLE [dbo].[ChannelMembers](
                	[ChannelId] [uniqueidentifier] NOT NULL,
                	[MemberId] [uniqueidentifier] NOT NULL,
                    [LastReadMessageId] [uniqueidentifier] NULL,
                    [IsMuted] [bit] NOT NULL DEFAULT(0),
                 CONSTRAINT [PK_ChannelMembers] PRIMARY KEY CLUSTERED ([MemberId] ASC, [ChannelId] ASC)
                 WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) 
                 ON [PRIMARY]
                
                ALTER TABLE [dbo].[ChannelMembers]  WITH CHECK ADD  CONSTRAINT [FK_ChannelMembers_Channels_ChannelId] FOREIGN KEY([ChannelId])
                REFERENCES [dbo].[Channels] ([Id])
                
                ALTER TABLE [dbo].[ChannelMembers] CHECK CONSTRAINT [FK_ChannelMembers_Channels_ChannelId]
                
                ALTER TABLE [dbo].[ChannelMembers]  WITH CHECK ADD  CONSTRAINT [FK_ChannelMembers_Members_MemberId] FOREIGN KEY([MemberId])
                REFERENCES [dbo].[Members] ([Id])
                
                ALTER TABLE [dbo].[ChannelMembers] CHECK CONSTRAINT [FK_ChannelMembers_Members_MemberId]

                ALTER TABLE [dbo].[ChannelMembers]  WITH CHECK ADD  CONSTRAINT [FK_ChannelMembers_Messages_LastReadMessageId] FOREIGN KEY([LastReadMessageId])
                REFERENCES [dbo].[Messages] ([Id])

                ALTER TABLE [dbo].[ChannelMembers] CHECK CONSTRAINT [FK_ChannelMembers_Messages_LastReadMessageId]

                -- Create Attachments table
                CREATE TABLE [dbo].[Attachments](
                	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
                	[ContentType] [nvarchar](max) NULL,
                	[Created] [datetimeoffset](7) NOT NULL,
                	[FileName] [nvarchar](max) NULL,
                	[MessageId] [uniqueidentifier] NULL,
                	[Size] [bigint] NOT NULL)

                ALTER TABLE [dbo].[Attachments]  WITH CHECK ADD  CONSTRAINT [FK_Attachments_Messages_MessageId] FOREIGN KEY([MessageId])
                REFERENCES [dbo].[Messages] ([Id])
                
                ALTER TABLE [dbo].[Attachments] CHECK CONSTRAINT [FK_Attachments_Messages_MessageId]
            ");
        }

        protected override void Down()
        {
            Execute(@"
                DROP TABLE [dbo].[Settings]
                DROP TABLE [dbo].[ChannelMembers]
                DROP TABLE [dbo].[Clients]
                DROP TABLE [dbo].[Notifications]
                DROP TABLE [dbo].[Messages]
                DROP TABLE[dbo].[Attachments]
                DROP TABLE [dbo].[Channels]
                DROP TABLE [dbo].[Members]       
            ");
        }
    }
}