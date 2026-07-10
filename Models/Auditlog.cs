using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LOCPS.Enums;

namespace LOCPS.Models
{
    public class Auditlog
    {
        [Key]
        public int Auditid { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public Actions Actions { get; set; } = Actions.Created;
        [StringLength(50)]
        public string EntityId { get; set; } = string.Empty;
        [StringLength(50)]
        public string Oldvalue { get; set; } = string.Empty;
        [StringLength(50)]

        public string Newvalue { get; set; } = string.Empty;

        public DateTime? Timestamp { get; set; }
        [StringLength(50)]
        public string IpAddress { get; set; } = string.Empty;
        [StringLength(50)]
        public string UserAgent { get; set; } = string.Empty;







    }
}
