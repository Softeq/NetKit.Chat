// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.Notification;

namespace Softeq.NetKit.Chat.Infrastructure.Storage.Sql.Repositories
{
    public interface INotificationRepository
    {
        Task AddNotificationAsync(Notification notification);
        Task DeletNotificationAsync(Guid notificationId);
        Task<Notification> GetNotificationByIdAsync(Guid notificationId);
        Task<List<Notification>> GetMemberNotificationsAsync(Guid memberId);
    }
}