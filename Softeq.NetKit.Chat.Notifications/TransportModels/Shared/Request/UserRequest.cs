namespace Softeq.NetKit.Chat.Notifications.TransportModels.Shared.Request
{
    public class UserRequest
    {
        public UserRequest(string userId)
        {
            UserId = userId;
        }
        public string UserId { get; set; }
    }
}
