using LOCPS.Models;

namespace LOCPS.Repositories.Interfaces
{
    public interface  INotificationRepository : IGenericRepository<Notification>
    {
        Task<Notification> CreateNotificationAsync(Notification Notification);

        // Returns ALL notifications for a user (was previously returning only the first one)
        Task<IEnumerable<Notification>> GetNotificationByUserIdAsync(int userId);

        Task<bool> MarkAsReadAsync(Notification Notification);
    }
}
