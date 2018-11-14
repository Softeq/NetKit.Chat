## Add Client (AddClientAsync)

#### Request:
	{
	 "SaasUserId": "string",
	 "UserName": "string",
	 "ConnectionId": "string",
	 "UserAgent": "string"
	}
#### Response:
	{
	 Id : Guid,
	 "ConnectionClientId": "string",
	 "UserName": "string",
	 "SaasUserId": "string"
	}
	
## Delete Client (DeleteClientAsync)
#### Request:
	{
	 "ClientConnectionId": "string"
	}

## Add Message (AddMessageAsync)
#### Request:
	{
	 "SaasUserId": "string",
	 "Body": "string",
	 ChannelId: Guid,
	 Type: MessageType (Enum: [Default, Notification]),
	 "ImageUrl": "string"
	}
#### Response:
	{
	 Id: Guid,
	 ChannelId: Guid,
	 Sender: MemberSummary
		 {
		  Id: Guid,
		  "SaasUserId", "string",
		  "UserName": "string",
		  Role: UserRole (Enum: [User = 0, Admin = 1]),
		  Status: UserRole (Enum: [User = 0, Admin = 1]),
		  IsAfk: bool,
		  LastActivity: DateTimeOffset,
		  "Email": "string",
		  "AvatarUrl": "string"
		 },
	 "Body": "string",
	 Created: DateTimeOffset,
	 Updated: DateTimeOffset,
	 Type: MessageType (Enum: [Default, Notification]),
	 IsRead: bool,
	 "ImageUrl": "string"
	}

## Delete Message (DeleteMessageAsync)
#### Request:
	{
	 "SaasUserId": "string",
	 MessageId: Guid
	}

## Update Message (UpdateMessageAsync)
#### Request:
	{
	 "SaasUserId": "string",
	 MessageId: Guid,
	 "Body": "string"
	}

## Add Message Attachment (AddMessageAttachmentAsync)
#### Request:
	{
	 "SaasUserId": "string",
	 MessageId: Guid,
	 Content: Stream,
	 "Extension": "string",
	 "ContentType": "string",
	 Size: long
	}

## Delete Message Attachment (DeleteMessageAttachmentAsync)
#### Request:
	{
	 "SaasUserId": "string",
	 MessageId: Guid,
	 AttachmentId: Guid
	}

## Mark As Read Message (MarkAsReadMessageAsync)
#### Request:
	{
	 "SaasUserId": "string",
	 ChannelId: Guid,
	 MessageId: Guid
	}

## Join To Channel (JoinToChannelAsync)
#### Request:
	{
	 "SaasUserId": "string",
	 ChannelId: Guid
	}
	
## Invite Member (InviteMemberAsync)
#### Request:
	{
	 "SaasUserId": "string",
	 ChannelId: Guid,
	 MemberId: Guid
	}
	
## Invite Members (InviteMembersAsync)
#### Request:
	{
	 "SaasUserId": "string",
	 ChannelId: Guid,
	 "InvitedMembers": Array[string]
	}

## Create Channel (CreateChannelAsync)
#### Request:
	{
	 "SaasUserId": "string",
	 "Name": "string",
	 "Description": "string",
	 "WelcomeMessage": "string",
	 Type: ChannelType (Enum: [Public,Private]),
	 AllowedMembers: Array[string],
	 "PhotoUrl": "string"
	}
#### Response:
	{
	 Id: Guid,
	 Created: DateTimeOffset,
	 Updated: DateTimeOffset,
	 UnreadMessagesCount: int,
	 Name: string,
	 IsClosed: bool,
	 IsMuted: bool,
	 CreatorId: Guid,
	 Creator: MemberSummary
		 {
		  Id: Guid,
		  "SaasUserId", "string",
		  "UserName": "string",
		  Role: UserRole (Enum: [User = 0, Admin = 1]),
		  Status: UserStatus(Active, Inactive, Offline),
		  IsAfk: bool,
		  LastActivity: DateTimeOffset,
		  "Email": "string",
		  "AvatarUrl": "string"
		 },
	"CreatorSaasUserId": "string",
	"Description": "string",
	"WelcomeMessage": "string",
	Type: ChannelType (Enum: [Public,Private]),
	LastMessage: MessageResponse
		{
		 Id: Guid,
		 ChannelId: Guid,
		 Sender: MemberSummary
			 {
			  Id: Guid,
			  "SaasUserId", "string",
			  "UserName": "string",
			  Role: UserRole (Enum: [User = 0, Admin = 1]),	
			  Status: UserRole (Enum: [User = 0, Admin = 1]),
			  IsAfk: bool,
			  LastActivity: DateTimeOffset,
			  "Email": "string",
			  "AvatarUrl": "string"
			 },
		 "Body": "string",
		 Created: DateTimeOffset,
		 Updated: DateTimeOffset,
		 Type: MessageType (Enum: [Default, Notification]),
		 IsRead: Bool,
		 "ImageUrl": "string"
		},
	"PhotoUrl": "string"
	}

## Update Channel (UpdateChannelAsync)
#### Request:
	{
	 "SaasUserId": "string",
	 ChannelId: Guid,
	 "Name": "string",
	 "Topic": "string",
	 "WelcomeMessage": "string",	
	 "PhotoUrl": "string"
	}
#### Response:
	{
	 Id: Guid,
	 Created: DateTimeOffset,
	 Updated: DateTimeOffset,
	 UnreadMessagesCount: int,
	 Name: string,
	 IsClosed: bool,
	 IsMuted: bool,
	 CreatorId: Guid,
	 Creator: MemberSummary
		 {
		  Id: Guid,
		  "SaasUserId", "string",
		  "UserName": "string",
		  Role: UserRole (Enum: [User = 0, Admin = 1]),
		  Status: UserRole (Enum: [User = 0, Admin = 1]),
		  IsAfk: bool,
		  LastActivity: DateTimeOffset,
		  "Email": "string",
		  "AvatarUrl": "string"
		 },
	"CreatorSaasUserId": "string",
	"Description": "string",
	"WelcomeMessage": "string",
	Type: ChannelType (Enum: [Public,Private]),
	LastMessage: MessageResponse
		{
		 Id: Guid,
		 ChannelId: Guid,
		 Sender: MemberSummary
			 {
			  Id: Guid,
			  "SaasUserId", "string",
			  "UserName": "string",
			  Role: UserRole (Enum: [User = 0, Admin = 1]),	
			  Status: UserRole (Enum: [User = 0, Admin = 1]),
			  IsAfk: bool,
			  LastActivity: DateTimeOffset,
			  "Email": "string",
			  "AvatarUrl": "string"
			 },
		 "Body": "string",
		 Created: DateTimeOffset,
		 Updated: DateTimeOffset,
		 Type: MessageType (Enum: [Default, Notification]),
		 IsRead: Bool,
		 "ImageUrl": "string"
		},
	"PhotoUrl": "string"
	}

## Close Channel (CloseChannelAsync)
#### Request:
	{
	 "SaasUserId": "string",
	 "ChannelId": Guid
	}
## Leave Channel (LeaveChannelAsync)
#### Request:
	{
	 "SaasUserId": "string",
	 "ChannelId": Guid
	}

## Mute Channel (MuteChannelAsync)
#### Request:
	{
	 "SaasUserId": "string",
	 "ChannelId": Guid
	}


	
