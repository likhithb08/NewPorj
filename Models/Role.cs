using System.ComponentModel.DataAnnotations;
using LOCPS.Enums;
namespace LOCPS.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        public Roles Roles { get; set; } = Roles.Customer ; // Instead of holding null It holds an empty value that can be overriden later
        [StringLength(250)]
        public string? RoleDescription { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // this is a property known as  Navigation property - Collection Navigation Property for Users as One role can have many users
        public ICollection<User> Users { get; set; } =new List<User>();

        //this is aproperty known as a Navigation property - Collection one as well for Permissions as one role can have multiple permissions
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }
}
