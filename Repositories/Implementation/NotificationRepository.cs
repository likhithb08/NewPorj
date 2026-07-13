using LOCPS.Data;
using LOCPS.Models;
using LOCPS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOCPS.Repositories.Implementation
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private readonly AppDbContext _context;
        public NotificationRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Notification> CreateNotificationAsync(Notification Notification)
        {
            await _context.Notifications.AddAsync(Notification);
            await _context.SaveChangesAsync();
            return Notification;
        }
        public async Task<Notification?> GetNotificationByUserIdAsync(int UserId)
        {
            return await _context.Notifications.FirstOrDefaultAsync(n => n.UserId == UserId);
        }
        public async Task<bool> MarkAsReadAsync( Notification notification)
        {
            if (notification == null) return false;
            if (!notification.IsRead)
            {
                notification.IsRead = true;
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();
            }
            return true;
            
        }

    }
}
