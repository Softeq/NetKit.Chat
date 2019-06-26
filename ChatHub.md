# Hub Commands

## Add Client (AddClientAsync)
###### Request:

```json 
NODATA
```
	
###### Response:

```json 
{
  "Id" : "Guid",
  "MemberId" : "Guid",
  "ConnectionClientId": "string",
  "UserName": "string",
  "SaasUserId": "string"
}
```
	
## Delete Client (DeleteClientAsync)
###### Request:

```json 
NODATA
```

###### Response:

```json 
NODATA
```

## Add Message (AddMessageAsync)
###### Request:

```json 
{
  "RequestId": "string",
  "ClientConnectionId": "string",
  "Body": "string",
  "ChannelId": "Guid",
  "Type": "MessageType" (Enum: [Default, Forward]),
  "ImageUrl": "string",
  "ForwardedMessageId": "Guid"
}
```

###### Response:

```json 
{
  "Id": "Guid",
  "ChannelId": "Guid",
  "Sender": "MemberSummary"
	{
	  "Id": "Guid",
	  "SaasUserId": "string",
	  "UserName": "string",
	  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),
	  "Status": "UserStatus" (Enum: [Offline = 0, Online = 1]),
	  "IsActive": "bool",
	  "LastActivity": "DateTimeOffset",
	  "Email": "string",
	  "AvatarUrl": "string"
	},
  "Body": "string",
  "Created": "DateTimeOffset",
  "Updated": "DateTimeOffset",
  "Type": "MessageType" (Enum: [Default, Forward, System]),
  "IsRead": "bool",
  "ImageUrl": "string",
  "ForwardedMessage": "ForwardMessageResponse"
	{
	  "Id": "Guid",
	  "Body": "string",
	  "ChannelId": "Guid",
	  "OwnerId": "Guid",
	  "Channel": "ChannelSummaryResponse"
	   {
	     ...
	   },
	  "Owner": "MemberSummaryResponse"
	   {
	     ...
	   },
	  "Created": "DateTimeOffset"
	}
}
```

## Delete Message (DeleteMessageAsync)
###### Request:

```json 
{
  "RequestId": "string",
  "MessageId": "Guid"
}
```

###### Response:

```json 
NODATA
```

## Update Message (UpdateMessageAsync)
###### Request:

```json 
{
  "RequestId": "string",
  "MessageId": "Guid",
  "Body": "string"
}
```

###### Response:

```json 
NODATA
```

## Mark As Read Message (MarkAsReadMessageAsync)
###### Request:

```json 
{
  "RequestId": "string",
  "ChannelId": "Guid",
  "MessageId": "Guid"
}
```

###### Response:

```json 
NODATA
```

## Invite Member (InviteMemberAsync)
###### Request:

```json 
{
  "RequestId": "string",
  "ChannelId": "Guid",
  "MemberId": "Guid"
}
```

###### Response:

```json 
NODATA
```
	
## Invite Multiple Members (InviteMultipleMembersAsync)
###### Request:

```json 
{
  "RequestId": "string",
  "ChannelId": "Guid",
  "InvitedMembersIds": "Array[string]"
}
```

###### Response:

```json 
NODATA
```

## Delete Member (DeleteMemberAsync)
###### Request:

```json 
{
  "RequestId": "string",
  "ChannelId": "Guid",
  "MemberId": "Guid"
}
```

###### Response:

```json 
NODATA
```

## Create Channel (CreateChannelAsync)
###### Request:

```json 
{
  "RequestId": "string",
  "Name": "string",
  "Description": "string",
  "WelcomeMessage": "string",
  "Type": "ChannelType" (Enum: [Public, Private]),
  "AllowedMembers": "Array[string]",
  "PhotoUrl": "string"
}
```

###### Response:

```json 
{
  "Id": "Guid",
  "Created": "DateTimeOffset",
  "Updated": "DateTimeOffset",
  "UnreadMessagesCount": "int",
  "Name": "string",
  "IsClosed": "bool",
  "IsMuted": "bool",
  "IsPinned": "bool",
  "Members": "Array[MemberSummaryResponse]"
  {
    [0]: 
	{
	  "Id": "Guid",
	  "SaasUserId": "string",
	  "UserName": "string",
	  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),
	  "Status": "UserStatus" (Enum: [Offline = 0, Online = 1]),
	  "IsActive": "bool",
	  "LastActivity": "DateTimeOffset",
	  "Email": "string",
	  "AvatarUrl": "string"
	},
	...
  },
  "Description": "string",
  "WelcomeMessage": "string",
  "Type": "ChannelType" (Enum: [Public, Private]),
  "LastMessage": "MessageResponse"
	{
	 "Id": "Guid",
	 "ChannelId": "Guid",
	 "Sender": "MemberSummary"
	  {
        ...
	  },
     "Body": "string",
     "Created": "DateTimeOffset",
     "Updated": "DateTimeOffset",
     "Type": "MessageType" (Enum: [Default, Forward, System]),
     "IsRead": "bool",
     "ImageUrl": "string",
     "ForwardedMessage": "ForwardMessageResponse"
	  {
	    "Id": "Guid",
	    "Body": "string",
	    "ChannelId": "Guid",
	    "OwnerId": "Guid",
	    "Channel": "ChannelSummaryResponse"
	     {
	       ...
	     },
	    "Owner": "MemberSummaryResponse"
	     {
	       ...
	     },
	    "Created": "DateTimeOffset"
	  }
	},
  "PhotoUrl": "string"
}
```

## Create Direct Channel (CreateDirectChannelAsync)
###### Request:

```json 
{
  "RequestId": "string",
  "MemberId": "Guid"
}
```

###### Response:

```json 
{
  "Id": "Guid",
  "Created": "DateTimeOffset",
  "Updated": "DateTimeOffset",
  "UnreadMessagesCount": "int",
  "Name": "string",
  "IsClosed": "bool",
  "IsMuted": "bool",
  "IsPinned": "bool",
  "Members": "Array[MemberSummaryResponse]"
  {
    [0]: 
	{
	  "Id": "Guid",
	  "SaasUserId": "string",
	  "UserName": "string",
	  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),
	  "Status": "UserStatus" (Enum: [Offline = 0, Online = 1]),
	  "IsActive": "bool",
	  "LastActivity": "DateTimeOffset",
	  "Email": "string",
	  "AvatarUrl": "string"
	},
	...
  },
  "Description": "string",
  "WelcomeMessage": "string",
  "Type": "ChannelType" (Enum: [Public, Private]),
  "LastMessage": "MessageResponse"
	{
	 "Id": "Guid",
	 "ChannelId": "Guid",
	 "Sender": "MemberSummary"
	  {
        ...
	  },
     "Body": "string",
     "Created": "DateTimeOffset",
     "Updated": "DateTimeOffset",
     "Type": "MessageType" (Enum: [Default, Forward, System]),
     "IsRead": "bool",
     "ImageUrl": "string",
     "ForwardedMessage": "ForwardMessageResponse"
	  {
	    "Id": "Guid",
	    "Body": "string",
	    "ChannelId": "Guid",
	    "OwnerId": "Guid",
	    "Channel": "ChannelSummaryResponse"
	     {
	       ...
	     },
	    "Owner": "MemberSummaryResponse"
	     {
	       ...
	     },
	    "Created": "DateTimeOffset"
	  }
	},
  "PhotoUrl": "string"
}
```


## Update Channel (UpdateChannelAsync)
###### Request:

```json 
{
  "RequestId": "string",
  "ChannelId": "Guid",
  "Name": "string",
  "Description": "string",
  "WelcomeMessage": "string",
  "PhotoUrl": "string"
}
```

###### Response:

```json 
{
  "Id": "Guid",
  "Created": "DateTimeOffset",
  "Updated": "DateTimeOffset",
  "UnreadMessagesCount": "int",
  "Name": "string",
  "IsClosed": "bool",
  "IsMuted": "bool",
  "IsPinned": "bool",
  "Members": "Array[MemberSummaryResponse]"
  {
    [0]: 
	{
	  "Id": "Guid",
	  "SaasUserId": "string",
	  "UserName": "string",
	  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),
	  "Status": "UserStatus" (Enum: [Offline = 0, Online = 1]),
	  "IsActive": "bool",
	  "LastActivity": "DateTimeOffset",
	  "Email": "string",
	  "AvatarUrl": "string"
	},
	...
  },
  "Description": "string",
  "WelcomeMessage": "string",
  "Type": "ChannelType" (Enum: [Public, Private]),
  "LastMessage": "MessageResponse"
	{
	 "Id": "Guid",
	 "ChannelId": "Guid",
	 "Sender": "MemberSummary"
	  {
        ...
	  },
     "Body": "string",
     "Created": "DateTimeOffset",
     "Updated": "DateTimeOffset",
     "Type": "MessageType" (Enum: [Default, Forward, System]),
     "IsRead": "bool",
     "ImageUrl": "string",
     "ForwardedMessage": "ForwardMessageResponse"
	  {
	    "Id": "Guid",
	    "Body": "string",
	    "ChannelId": "Guid",
	    "OwnerId": "Guid",
	    "Channel": "ChannelSummaryResponse"
	     {
	       ...
	     },
	    "Owner": "MemberSummaryResponse"
	     {
	       ...
	     },
	    "Created": "DateTimeOffset"
	  }
	},
  "PhotoUrl": "string"
}
```

## Close Channel (CloseChannelAsync)
###### Request:

```json 
{
  "RequestId": "string",
  "ChannelId": "Guid"
}
```

###### Response:

```json 
NODATA
```

## Leave Channel (LeaveChannelAsync)
###### Request:

```json
{
  "RequestId": "string",
  "ChannelId": "Guid"
}
```

###### Response:

```json 
NODATA
```

## Mute Channel (MuteChannelAsync)
###### Request:

```json
{
  "RequestId": "string",
  "IsMuted": "bool",
  "ChannelId": "Guid"
}
```

###### Response:

```json 
NODATA
```
	
## Pin Channel (PinChannelAsync)
###### Request:

```json
{
  "ChannelId": Guid,
  "IsPinned": "bool",
  "ChannelId": "Guid"
}
```

###### Response:

```json 
NODATA
```

# Hub Events

## Channel Added (MemberJoined)
###### Response:
###### ChannelSummaryResponse:

```json 
{
  "Id": "Guid",
  "Created": "DateTimeOffset",
  "Updated": "DateTimeOffset",
  "UnreadMessagesCount": "int",
  "Name": "string",
  "IsClosed": "bool",
  "IsMuted": "bool",
  "IsPinned": "bool",
  "Members": "Array[MemberSummaryResponse]"
  {
    [0]: 
	{
	  "Id": "Guid",
	  "SaasUserId": "string",
	  "UserName": "string",
	  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),
	  "Status": "UserStatus" (Enum: [Offline = 0, Online = 1]),
	  "IsActive": "bool",
	  "LastActivity": "DateTimeOffset",
	  "Email": "string",
	  "AvatarUrl": "string"
	},
	...
  },
  "Description": "string",
  "WelcomeMessage": "string",
  "Type": "ChannelType" (Enum: [Public, Private]),
  "LastMessage": "MessageResponse"
	{
	 "Id": "Guid",
	 "ChannelId": "Guid",
	 "Sender": "MemberSummary"
	  {
        ...
	  },
     "Body": "string",
     "Created": "DateTimeOffset",
     "Updated": "DateTimeOffset",
     "Type": "MessageType" (Enum: [Default, Forward, System]),
     "IsRead": "bool",
     "ImageUrl": "string",
     "ForwardedMessage": "ForwardMessageResponse"
	  {
	    "Id": "Guid",
	    "Body": "string",
	    "ChannelId": "Guid",
	    "OwnerId": "Guid",
	    "Channel": "ChannelSummaryResponse"
	     {
	       ...
	     },
	    "Owner": "MemberSummaryResponse"
	     {
	       ...
	     },
	    "Created": "DateTimeOffset"
	  }
	},
  "PhotoUrl": "string"
}
```

## Channel Updated (ChannelUpdated)
###### Response:
###### ChannelSummaryResponse:

```json 
{
  "Id": "Guid",
  "Created": "DateTimeOffset",
  "Updated": "DateTimeOffset",
  "UnreadMessagesCount": "int",
  "Name": "string",
  "IsClosed": "bool",
  "IsMuted": "bool",
  "IsPinned": "bool",
  "Members": "Array[MemberSummaryResponse]"
  {
    [0]: 
	{
	  "Id": "Guid",
	  "SaasUserId": "string",
	  "UserName": "string",
	  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),
	  "Status": "UserStatus" (Enum: [Offline = 0, Online = 1]),
	  "IsActive": "bool",
	  "LastActivity": "DateTimeOffset",
	  "Email": "string",
	  "AvatarUrl": "string"
	},
	...
  },
  "Description": "string",
  "WelcomeMessage": "string",
  "Type": "ChannelType" (Enum: [Public, Private]),
  "LastMessage": "MessageResponse"
	{
	 "Id": "Guid",
	 "ChannelId": "Guid",
	 "Sender": "MemberSummary"
	  {
        ...
	  },
     "Body": "string",
     "Created": "DateTimeOffset",
     "Updated": "DateTimeOffset",
     "Type": "MessageType" (Enum: [Default, Forward, System]),
     "IsRead": "bool",
     "ImageUrl": "string",
     "ForwardedMessage": "ForwardMessageResponse"
	  {
	    "Id": "Guid",
	    "Body": "string",
	    "ChannelId": "Guid",
	    "OwnerId": "Guid",
	    "Channel": "ChannelSummaryResponse"
	     {
	       ...
	     },
	    "Owner": "MemberSummaryResponse"
	     {
	       ...
	     },
	    "Created": "DateTimeOffset"
	  }
	},
  "PhotoUrl": "string"
}
```

## Channel Closed (ChannelClosed)
###### Response:

```json
"ChannelId" : "Guid"
```

## Member Joined (MemberJoined)
###### Response:
###### ChannelSummaryResponse:
	
```json 
{
  "Id": "Guid",
  "Created": "DateTimeOffset",
  "Updated": "DateTimeOffset",
  "UnreadMessagesCount": "int",
  "Name": "string",
  "IsClosed": "bool",
  "IsMuted": "bool",
  "IsPinned": "bool",
  "Members": "Array[MemberSummaryResponse]"
  {
    [0]: 
	{
	  "Id": "Guid",
	  "SaasUserId": "string",
	  "UserName": "string",
	  "Role": "UserRole" (Enum: [User = 0, Admin = 1]),
	  "Status": "UserStatus" (Enum: [Offline = 0, Online = 1]),
	  "IsActive": "bool",
	  "LastActivity": "DateTimeOffset",
	  "Email": "string",
	  "AvatarUrl": "string"
	},
	...
  },
  "Description": "string",
  "WelcomeMessage": "string",
  "Type": "ChannelType" (Enum: [Public, Private]),
  "LastMessage": "MessageResponse"
	{
	 "Id": "Guid",
	 "ChannelId": "Guid",
	 "Sender": "MemberSummary"
	  {
        ...
	  },
     "Body": "string",
     "Created": "DateTimeOffset",
     "Updated": "DateTimeOffset",
     "Type": "MessageType" (Enum: [Default, Forward, System]),
     "IsRead": "bool",
     "ImageUrl": "string",
     "ForwardedMessage": "ForwardMessageResponse"
	  {
	    "Id": "Guid",
	    "Body": "string",
	    "ChannelId": "Guid",
	    "OwnerId": "Guid",
	    "Channel": "ChannelSummaryResponse"
	     {
	       ...
	     },
	    "Owner": "MemberSummaryResponse"
	     {
	       ...
	     },
	    "Created": "DateTimeOffset"
	  }
	},
  "PhotoUrl": "string"
}
```

## Member Left (MemberLeft)
###### Response:

```json
"ChannelId" : "Guid"
```

## Message Added (MessageAdded)
###### Response:
###### MessageResponse:

```json
{
  "Id": "Guid",
  "ChannelId": "Guid",
  "Sender": "MemberSummary"
   {
	 ...
   },
  "Body": "string",
  "Created": "DateTimeOffset",
  "Updated": "DateTimeOffset",
  "Type": "MessageType" (Enum: [Default, Forward, System]),
  "IsRead": "bool",
  "ImageUrl": "string",
  "ForwardedMessage": "ForwardMessageResponse"
  {
	"Id": "Guid",
	"Body": "string",
	"ChannelId": "Guid",
	"OwnerId": "Guid",
	"Channel": "ChannelSummaryResponse"
	 {
	   ...
	 },
	"Owner": "MemberSummaryResponse"
	 {
	   ...
	 },
	"Created": "DateTimeOffset"
  }
}
```

## Message Deleted (MessageDeleted)
###### Response:

```json
"MessageId" : "Guid"
```

```json
"ChannelId" : "Guid"
```

## Message Updated (MessageUpdated)
###### Response:
###### MessageResponse:

```json
{
  "Id": "Guid",
  "ChannelId": "Guid",
  "Sender": "MemberSummary"
   {
	 ...
   },
  "Body": "string",
  "Created": "DateTimeOffset",
  "Updated": "DateTimeOffset",
  "Type": "MessageType" (Enum: [Default, Forward, System]),
  "IsRead": "bool",
  "ImageUrl": "string",
  "ForwardedMessage": "ForwardMessageResponse"
  {
	"Id": "Guid",
	"Body": "string",
	"ChannelId": "Guid",
	"OwnerId": "Guid",
	"Channel": "ChannelSummaryResponse"
	 {
	   ...
	 },
	"Owner": "MemberSummaryResponse"
	 {
	   ...
	 },
	"Created": "DateTimeOffset"
  }
}
```

## Last Read Message Changed (LastReadMessageChanged)
###### Response:

```json
"ChannelId" : "Guid"
```

## Add Message Attachment (AttachmentAdded)
###### Response:

```json
"ChannelId" : "Guid"
```

## Delete Message Attachment (AttachmentDeleted)
###### Response:

```json
"ChannelId" : "Guid"
```

## Request Success (RequestSuccess)
###### Response:

```json
"RequestId": "string"
```

## Exception Occurred (ExceptionOccurred)
###### Response:

```json
"RequestId": "string"
```
	
