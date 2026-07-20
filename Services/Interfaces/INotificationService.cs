using LOCPS.Enums;
using LOCPS.Models;

namespace LOCPS.Services.Interfaces;

public interface INotificationService
{
    Task<Notification> CreateAsync(int userId, NotificationType type, string title, string message, int? relatedApplicationId = null);
    Task<IEnumerable<Notification>> GetForUserAsync(int userId, bool unreadOnly = false);
    Task MarkAsReadAsync(int notificationId);
}
