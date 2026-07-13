using LOCPS.Models;

namespace LOCPS.Repositories.Interfaces
{
    public interface  INotificationRepository : IGenericRepository<Notification>
    {
        Task<Notification> CreateNotificationAsync(Notification Notification);

        Task<Notification?> GetNotificationByUserIdAsync(int  userId);

        Task<bool> MarkAsReadAsync(Notification Notification);
    }
}
