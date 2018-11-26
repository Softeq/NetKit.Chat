using System;


namespace Softeq.NoName.Service.TransportModels.Setting.Response
{
    public class NotificationSettingResponse
    {
        // Fill in according to the notification settings of your application
        //public NotificationSettingValue TemplateLiked { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string SaasUserId { get; set; }
    }
}
