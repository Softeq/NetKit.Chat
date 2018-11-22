# Hub Commands
## Add Client (AddClientAsync)

#### Request:
	{
	 "UserName": "string",
	 "ConnectionId": "string",
	 "UserAgent": "string"
	}
#### Response:
	{
	 "Id" : "Guid",
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
	 "Body": "string",
	 "ChannelId": "Guid",
	 "Type": "MessageType" (Enum: [Default, Notification]),
	 "ImageUrl": "string"
	}
#### Response:
	{
	 "Id": "Guid",
	 "ChannelId": "Guid",
	 "Sender": "MemberSummary"
		 {
		  "Id": "Guid",
		  "SaasUserId": "string",
		  "UserName": "string",
		  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),
		  "Status": "UserRole" (Enum: [User = 0, Admin = 1]),
		  "IsAfk": "bool",
		  "LastActivity": "DateTimeOffset",
		  "Email": "string",
		  "AvatarUrl": "string"
		 },
	 "Body": "string",
	 "Created": "DateTimeOffset",
	 "Updated": "DateTimeOffset",
	 "Type": "MessageType" (Enum: [Default, Notification]),
	 "IsRead": "bool",
	 "ImageUrl": "string"
	}

## Delete Message (DeleteMessageAsync)
#### Request:
	{
	 "MessageId": "Guid"
	}

## Update Message (UpdateMessageAsync)
#### Request:
	{
	 "MessageId": "Guid",
	 "Body": "string"
	}

## Add Message Attachment (AddMessageAttachmentAsync)
#### Request:
	{
	 "MessageId": "Guid",
	 "Content": "Stream",
	 "Extension": "string",
	 "ContentType": "string",
	 "Size": "long"
	}

## Delete Message Attachment (DeleteMessageAttachmentAsync)
#### Request:
	{
	 "MessageId": "Guid",
	 "AttachmentId": "Guid"
	}

## Mark As Read Message (MarkAsReadMessageAsync)
#### Request:
	{
	 "ChannelId": "Guid",
	 "MessageId": "Guid"
	}

## Join To Channel (JoinToChannelAsync)
#### Request:
	{
	 "ChannelId": "Guid"
	}
	
## Invite Member (InviteMemberAsync)
#### Request:
	{
	 "ChannelId": "Guid",
	 "MemberId": "Guid"
	}
	
## Invite Multiple Members (InviteMultipleMembersAsync)
#### Request:
	{
	 "ChannelId": "Guid",
	 "InvitedMembers": "Array[string]"
	}

## Create Channel (CreateChannelAsync)
#### Request:
	{
	 "Name": "string",
	 "Description": "string",
	 "WelcomeMessage": "string",
	 "Type": "ChannelType" (Enum: [Public,Private]),
	 "AllowedMembers": "Array[string]",
	 "PhotoUrl": "string"
	}
#### Response:
	{
	 "Id": "Guid",
	 "Created": "DateTimeOffset",
	 "Updated": "DateTimeOffset",
	 "UnreadMessagesCount": "int",
	 "Name": "string",
	 "IsClosed": "bool",
	 "IsMuted": "bool",
	 "CreatorId": "Guid",
	 "Creator": "MemberSummary"
		 {
		  "Id": "Guid",
		  "SaasUserId", "string",
		  "UserName": "string",
		  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),
		  "Status": "UserStatus" (Enum: [Active, Inactive, Offline]),
		  "IsAfk": "bool",
		  "LastActivity": "DateTimeOffset",
		  "Email": "string",
		  "AvatarUrl": "string"
		 },
	"CreatorSaasUserId": "string",
	"Description": "string",
	"WelcomeMessage": "string",
	"Type": "ChannelType" (Enum: [Public,Private]),
	"LastMessage": "MessageResponse"
		{
		 "Id": "Guid",
		 "ChannelId": "Guid",
		 "Sender": "MemberSummary"
			 {
			  Id: Guid,
			  "SaasUserId", "string",
			  "UserName": "string",
			  "Role: UserRole" (Enum: [User = 0, Admin = 1]),	
			  "Status": "UserRole" (Enum: [User = 0, Admin = 1]),
			  "IsAfk": "bool",
			  "LastActivity": "DateTimeOffset",
			  "Email": "string",
			  "AvatarUrl": "string"
			 },
		 "Body": "string",
		 "Created": "DateTimeOffset",
		 "Updated": "DateTimeOffset",
		 "Type": "MessageType" (Enum: [Default, Notification]),
		 "IsRead": "Bool",
		 "ImageUrl": "string"
		},
	"PhotoUrl": "string"
	}

## Update Channel (UpdateChannelAsync)
#### Request:
	{
	 "ChannelId": "Guid",
	 "Name": "string",
	 "Topic": "string",
	 "WelcomeMessage": "string",	
	 "PhotoUrl": "string"
	}
#### Response:
	{
	 "Id": "Guid",
	 "Created": "DateTimeOffset",
	 "Updated": "DateTimeOffset",
	 "UnreadMessagesCount": "int",
	 "Name": "string",
	 "IsClosed": "bool",
	 "IsMuted": "bool",
	 "CreatorId": "Guid",
	 "Creator": "MemberSummary"
		 {
		  "Id": "Guid",
		  "SaasUserId", "string",
		  "UserName": "string",
		  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),
		  "Status": "UserRole" (Enum: [User = 0, Admin = 1]),
		  "IsAfk": "bool",
		  "LastActivity": "DateTimeOffset",
		  "Email": "string",
		  "AvatarUrl": "string"
		 },
	"CreatorSaasUserId": "string",
	"Description": "string",
	"WelcomeMessage": "string",
	"Type": "ChannelType" (Enum: [Public, Private]),
	"LastMessage": "MessageResponse"
		{
		 "Id": "Guid",
		 "ChannelId": "Guid",
		 "Sender": "MemberSummary"
			 {
			  "Id": "Guid",
			  "SaasUserId", "string",
			  "UserName": "string",
			  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),	
			  "Status": "UserRole" (Enum: [User = 0, Admin = 1]),
			  "IsAfk": "bool",
			  "LastActivity": "DateTimeOffset",
			  "Email": "string",
			  "AvatarUrl": "string"
			 },
		 "Body": "string",
		 "Created": "DateTimeOffset",
		 "Updated": "DateTimeOffset",
		 "Type": "MessageType" (Enum: [Default, Notification]),
		 "IsRead": "bool",
		 "ImageUrl": "string"
		},
	"PhotoUrl": "string"
	}

## Close Channel (CloseChannelAsync)
#### Request:
	{
	 "ChannelId": Guid
	}
## Leave Channel (LeaveChannelAsync)
#### Request:
	{
	 "ChannelId": Guid
	}

## Mute Channel (MuteChannelAsync)
#### Request:
	{
	 "ChannelId": Guid
	}
	
# Hub Events

## Channel Added (ChannelAdded)
#### Response:
##### Channel:
	{
	 "Id": "Guid",
	 "Created": "DateTimeOffset",
	 "Updated": "DateTimeOffset",
	 "UnreadMessagesCount": "int",
	 "Name": "string",
	 "IsClosed": "bool",
	 "IsMuted": "bool",
	 "CreatorId": "Guid",
	 "Creator": "MemberSummary"
		 {
		  "Id": "Guid",
		  "SaasUserId", "string",
		  "UserName": "string",
		  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),
		  "Status": "UserRole" (Enum: [User = 0, Admin = 1]),
		  "IsAfk": "bool",
		  "LastActivity": "DateTimeOffset",
		  "Email": "string",
		  "AvatarUrl": "string"
		 },
	"CreatorSaasUserId": "string",
	"Description": "string",
	"WelcomeMessage": "string",
	"Type": "ChannelType" (Enum: [Public, Private]),
	"LastMessage": "MessageResponse"
		{
		 "Id": "Guid",
		 "ChannelId": "Guid",
		 "Sender": "MemberSummary"
			 {
			  "Id": "Guid",
			  "SaasUserId", "string",
			  "UserName": "string",
			  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),	
			  "Status": "UserRole" (Enum: [User = 0, Admin = 1]),
			  "IsAfk": "bool",
			  "LastActivity": "DateTimeOffset",
			  "Email": "string",
			  "AvatarUrl": "string"
			 },
		 "Body": "string",
		 "Created": "DateTimeOffset",
		 "Updated": "DateTimeOffset",
		 "Type": "MessageType" (Enum: [Default, Notification]),
		 "IsRead": "bool",
		 "ImageUrl": "string"
		},
	"PhotoUrl": "string"
	}
## Channel Updated (ChannelUpdated)
#### Response:
##### Channel:
	{
	 "Id": "Guid",
	 "Created": "DateTimeOffset",
	 "Updated": "DateTimeOffset",
	 "UnreadMessagesCount": "int",
	 "Name": "string",
	 "IsClosed": "bool",
	 "IsMuted": "bool",
	 "CreatorId": "Guid",
	 "Creator": "MemberSummary"
		 {
		  "Id": "Guid",
		  "SaasUserId", "string",
		  "UserName": "string",
		  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),
		  "Status": "UserRole" (Enum: [User = 0, Admin = 1]),
		  "IsAfk": "bool",
		  "LastActivity": "DateTimeOffset",
		  "Email": "string",
		  "AvatarUrl": "string"
		 },
	"CreatorSaasUserId": "string",
	"Description": "string",
	"WelcomeMessage": "string",
	"Type": "ChannelType" (Enum: [Public, Private]),
	"LastMessage": "MessageResponse"
		{
		 "Id": "Guid",
		 "ChannelId": "Guid",
		 "Sender": "MemberSummary"
			 {
			  "Id": "Guid",
			  "SaasUserId", "string",
			  "UserName": "string",
			  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),	
			  "Status": "UserRole" (Enum: [User = 0, Admin = 1]),
			  "IsAfk": "bool",
			  "LastActivity": "DateTimeOffset",
			  "Email": "string",
			  "AvatarUrl": "string"
			 },
		 "Body": "string",
		 "Created": "DateTimeOffset",
		 "Updated": "DateTimeOffset",
		 "Type": "MessageType" (Enum: [Default, Notification]),
		 "IsRead": "bool",
		 "ImageUrl": "string"
		},
	"PhotoUrl": "string"
	}
## Channel Closed (ChannelClosed)
#### Response:
###### Channel:
	{
	 "Id": "Guid",
	 "Created": "DateTimeOffset",
	 "Updated": "DateTimeOffset",
	 "UnreadMessagesCount": "int",
	 "Name": "string",
	 "IsClosed": "bool",
	 "IsMuted": "bool",
	 "CreatorId": "Guid",
	 "Creator": "MemberSummary"
		 {
		  "Id": "Guid",
		  "SaasUserId", "string",
		  "UserName": "string",
		  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),
		  "Status": "UserRole" (Enum: [User = 0, Admin = 1]),
		  "IsAfk": "bool",
		  "LastActivity": "DateTimeOffset",
		  "Email": "string",
		  "AvatarUrl": "string"
		 },
	"CreatorSaasUserId": "string",
	"Description": "string",
	"WelcomeMessage": "string",
	"Type": "ChannelType" (Enum: [Public, Private]),
	"LastMessage": "MessageResponse"
		{
		 "Id": "Guid",
		 "ChannelId": "Guid",
		 "Sender": "MemberSummary"
			 {
			  "Id": "Guid",
			  "SaasUserId", "string",
			  "UserName": "string",
			  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),	
			  "Status": "UserRole" (Enum: [User = 0, Admin = 1]),
			  "IsAfk": "bool",
			  "LastActivity": "DateTimeOffset",
			  "Email": "string",
			  "AvatarUrl": "string"
			 },
		 "Body": "string",
		 "Created": "DateTimeOffset",
		 "Updated": "DateTimeOffset",
		 "Type": "MessageType" (Enum: [Default, Notification]),
		 "IsRead": "bool",
		 "ImageUrl": "string"
		},
	"PhotoUrl": "string"
	}

## Member Joined (MemberJoined)
#### Response:
###### Channel:
	{
	 "Id": "Guid",
	 "Created": "DateTimeOffset",
	 "Updated": "DateTimeOffset",
	 "UnreadMessagesCount": "int",
	 "Name": "string",
	 "IsClosed": "bool",
	 "IsMuted": "bool",
	 "CreatorId": "Guid",
	 "Creator": "MemberSummary"
		 {
		  "Id": "Guid",
		  "SaasUserId", "string",
		  "UserName": "string",
		  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),
		  "Status": "UserRole" (Enum: [User = 0, Admin = 1]),
		  "IsAfk": "bool",
		  "LastActivity": "DateTimeOffset",
		  "Email": "string",
		  "AvatarUrl": "string"
		 },
	"CreatorSaasUserId": "string",
	"Description": "string",
	"WelcomeMessage": "string",
	"Type": "ChannelType" (Enum: [Public, Private]),
	"LastMessage": "MessageResponse"
		{
		 "Id": "Guid",
		 "ChannelId": "Guid",
		 "Sender": "MemberSummary"
			 {
			  "Id": "Guid",
			  "SaasUserId", "string",
			  "UserName": "string",
			  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),	
			  "Status": "UserRole" (Enum: [User = 0, Admin = 1]),
			  "IsAfk": "bool",
			  "LastActivity": "DateTimeOffset",
			  "Email": "string",
			  "AvatarUrl": "string"
			 },
		 "Body": "string",
		 "Created": "DateTimeOffset",
		 "Updated": "DateTimeOffset",
		 "Type": "MessageType" (Enum: [Default, Notification]),
		 "IsRead": "bool",
		 "ImageUrl": "string"
		},
	"PhotoUrl": "string"
	}
###### Member:
	 {
	  "Id": "Guid",
	  "SaasUserId": "string",
	  "UserName": "string",
	  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),
	  "Status": "UserRole" (Enum: [User = 0, Admin = 1]),
	  "IsAfk": "bool",
	  "LastActivity": "DateTimeOffset",
	  "Email": "string",
	  "AvatarUrl": "string"
	 }

## Member Left (MemberLeft)
#### Response:
###### Member:
	 {
	  "Id": "Guid",
	  "SaasUserId": "string",
	  "UserName": "string",
	  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),
	  "Status": "UserRole" (Enum: [User = 0, Admin = 1]),
	  "IsAfk": "bool",
	  "LastActivity": "DateTimeOffset",
	  "Email": "string",
	  "AvatarUrl": "string"
	 }
###### Channel_Id:
	{
	 "Id": "Guid",
	}
## Message Added (MessageAdded)
#### Response:
###### Message:
	{
	 "Id": "Guid",
	 "ChannelId": "Guid",
	 "Sender": "MemberSummary"
		 {
		  "Id": "Guid",
		  "SaasUserId": "string",
		  "UserName": "string",
		  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),
		  "Status": "UserRole" (Enum: [User = 0, Admin = 1]),
		  "IsAfk": "bool",
		  "LastActivity": "DateTimeOffset",
		  "Email": "string",
		  "AvatarUrl": "string"
		 },
	 "Body": "string",
	 "Created": "DateTimeOffset",
	 "Updated": "DateTimeOffset",
	 "Type": "MessageType" (Enum: [Default, Notification]),
	 "IsRead": "bool",
	 "ImageUrl": "string"
	}

## Message Deleted (MessageDeleted)
#### Response:
###### Message_Id
	{
	 "Id": "Guid",
	}
###### Channel:
	{
	 "Id": "Guid",
	 "Created": "DateTimeOffset",
	 "Updated": "DateTimeOffset",
	 "UnreadMessagesCount": "int",
	 "Name": "string",
	 "IsClosed": "bool",
	 "IsMuted": "bool",
	 "CreatorId": "Guid",
	 "Creator": "MemberSummary"
		 {
		  "Id": "Guid",
		  "SaasUserId", "string",
		  "UserName": "string",
		  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),
		  "Status": "UserRole" (Enum: [User = 0, Admin = 1]),
		  "IsAfk": "bool",
		  "LastActivity": "DateTimeOffset",
		  "Email": "string",
		  "AvatarUrl": "string"
		 },
	"CreatorSaasUserId": "string",
	"Description": "string",
	"WelcomeMessage": "string",
	"Type": "ChannelType" (Enum: [Public, Private]),
	"LastMessage": "MessageResponse"
		{
		 "Id": "Guid",
		 "ChannelId": "Guid",
		 "Sender": "MemberSummary"
			 {
			  "Id": "Guid",
			  "SaasUserId", "string",
			  "UserName": "string",
			  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),	
			  "Status": "UserRole" (Enum: [User = 0, Admin = 1]),
			  "IsAfk": "bool",
			  "LastActivity": "DateTimeOffset",
			  "Email": "string",
			  "AvatarUrl": "string"
			 },
		 "Body": "string",
		 "Created": "DateTimeOffset",
		 "Updated": "DateTimeOffset",
		 "Type": "MessageType" (Enum: [Default, Notification]),
		 "IsRead": "bool",
		 "ImageUrl": "string"
		},
	"PhotoUrl": "string"
	}
	
## Message Updated (MessageUpdated)
#### Response:
###### Message:
	{
	 "Id": "Guid",
	 "ChannelId": "Guid",
	 "Sender": "MemberSummary"
		 {
		  "Id": "Guid",
		  "SaasUserId": "string",
		  "UserName": "string",
		  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),
		  "Status": "UserRole" (Enum: [User = 0, Admin = 1]),
		  "IsAfk": "bool",
		  "LastActivity": "DateTimeOffset",
		  "Email": "string",
		  "AvatarUrl": "string"
		 },
	 "Body": "string",
	 "Created": "DateTimeOffset",
	 "Updated": "DateTimeOffset",
	 "Type": "MessageType" (Enum: [Default, Notification]),
	 "IsRead": "bool",
	 "ImageUrl": "string"
	}

## Attachment Added (AttachmentAdded)
#### Response:
###### Channel_Name
	{
	 "Name": "string"
	}
## Attachment Deleted (AttachmentDeleted)
#### Response:
###### Channel_Name
	{
	 "Name": "string"
	}
## Last Read Message Changed (LastReadMessageChanged)
#### Response:
###### Channel_Name
	{
	 "Name": "string"
	}

## Request Success (RequestSuccess)
#### Response:
###### Request_Id:
	{
         "RequestId": "string"
	}
## Exception Occurred (ExceptionOccurred)
#### Response:
###### Request_Id:
	{
         "RequestId": "string"
	}
## Access Token Expired (AccessTokenExpired)
#### Response:
###### Request_Id:
	{
         "RequestId": "string"
	}
	
