// Developed by Softeq Development Corporation
// http://www.softeq.com

using SimpleMigrations;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Migrations
{
    [Migration(20181123100500, "ForwardMessages table")]
    public class ForwardingMessages : Migration
    {
        protected override void Up()
        {
            Execute(@"
                CREATE TABLE [dbo].[ForwardMessages](
	                [Id] [uniqueidentifier] NOT NULL,
	                [Body] [nvarchar](max) NULL,
	                [Created] [datetimeoffset](7) NOT NULL,
	                [ChannelId] [uniqueidentifier] NOT NULL,
	                [OwnerId] [uniqueidentifier] NULL,
                PRIMARY KEY CLUSTERED 
                (
	                [Id] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

                ALTER TABLE [dbo].[ForwardMessages]  WITH CHECK ADD  CONSTRAINT [FK_ForwardMessages_Channels] FOREIGN KEY([ChannelId])
                REFERENCES [dbo].[Channels] ([Id])

                ALTER TABLE [dbo].[ForwardMessages] CHECK CONSTRAINT [FK_ForwardMessages_Channels]

                ALTER TABLE [dbo].[ForwardMessages]  WITH CHECK ADD  CONSTRAINT [FK_ForwardMessages_Members] FOREIGN KEY([OwnerId])
                REFERENCES [dbo].[Members] ([Id])

                ALTER TABLE [dbo].[ForwardMessages] CHECK CONSTRAINT [FK_ForwardMessages_Members]

                ALTER TABLE [dbo].[Messages] ADD ForwardId uniqueidentifier NULL
            ");
        }

        protected override void Down()
        {
            Execute(@"DROP TABLE [dbo].[ForwardMessages]");
        }
    }
}
