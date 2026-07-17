using LOCPS.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOCPS.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public NotificationType NotificationType { get; set; }
        [StringLength(50)]
        public string Title { get; set; } = string.Empty;

        [StringLength(300)]
        public string Message { get; set; } = string.Empty;
        [ForeignKey(nameof(RelatedApplication))]
        public int RelatedApplicationId { get; set; }
        public LoanApplication RelatedApplication { get; set; } = null!;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ReadDate { get; set; } = DateTime.UtcNow;
    }
}

