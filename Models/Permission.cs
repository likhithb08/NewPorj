using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOCPS.Models
{
    public class Permission
    {
        [Key]
        public int PermissionId { get; set; }

        [Required]
        [StringLength(100)]
        // string.Empty to hold Empty value instead of null
        public string PermissionName { get; set; } = string.Empty;
        [StringLength(250)]
        public string? Description { get; set; }
        [ForeignKey(nameof(Role))]
        public int RoleId { get; set; }

        public Role? Role { get; set; } = null;

    }
}
