using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOCPS.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;// Instead of holding null It holds an empty value that can be overriden later

        [Required]
        [EmailAddress]
        public string Email { get; set; } 

        [Required]
        [StringLength(50)]
        public string FullName { get; set; } = string.Empty;// Instead of holding null It holds an empty value that can be overriden later

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;// Instead of holding null It holds an empty value that can be overriden later

        [Required]
        [Phone]
        [StringLength(15)]
        public string PhoneNumber { get; set; } = string.Empty; // Instead of holding null It holds an empty value that can be overriden later

        [ForeignKey(nameof(Models.Role))]
        public int RoleId { get; set; }
        //Navigation property - Refernace type navigation Property for roles as one use can have only one role
        public Role? Role { get; set; } = null!;

        public bool IsActive { get; set; } = true;
        

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginDate { get; set; }

    }
}
