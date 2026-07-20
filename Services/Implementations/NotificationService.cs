using LOCPS.Common;
using LOCPS.Enums;
using LOCPS.Models;
using LOCPS.Repositories.Interfaces;
using LOCPS.Services.Interfaces;

namespace LOCPS.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;

    public NotificationService(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Notification> CreateAsync(int userId, NotificationType type, string title, string message, int? relatedApplicationId = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            NotificationType = type,
            Title = title,
            Message = message,
            RelatedApplicationId = relatedApplicationId ?? 0,
            IsRead = false,
            CreatedDate = DateTime.UtcNow
        };

        return await _repository.CreateNotificationAsync(notification)
            ?? throw new ServiceException("Failed to create notification.");
    }

    public async Task<IEnumerable<Notification>> GetForUserAsync(int userId, bool unreadOnly = false)
    {
        var notifications = await _repository.GetNotificationByUserIdAsync(userId);
        return unreadOnly ? notifications.Where(n => !n.IsRead) : notifications;
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        var notification = await _repository.GetByIdAsync(notificationId)
            ?? throw new ServiceException("Notification not found.", 404);
        notification.IsRead = true;
        await _repository.MarkAsReadAsync(notification);
    }
}
