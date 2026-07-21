using Microsoft.AspNetCore.Mvc;
using LOCPS.Services.Interfaces;
using LOCPS.Models;
using LOCPS.Enums;
using LOCPS.Common;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace LOCPS.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
            {
                // Fallback for demo preview
                userId = 3; 
            }

            var notifications = await _notificationService.GetForUserAsync(userId);
            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllRead()
        {
            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdStr, out var userId))
                {
                    var notifications = await _notificationService.GetForUserAsync(userId, unreadOnly: true);
                    foreach (var notif in notifications)
                    {
                        await _notificationService.MarkAsReadAsync(notif.NotificationId);
                    }
                    TempData["Success"] = "All notifications marked as read.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> MarkRead(int id)
        {
            try
            {
                await _notificationService.MarkAsReadAsync(id);
                TempData["Success"] = "Notification marked as read.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
