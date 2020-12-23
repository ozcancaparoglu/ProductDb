using ProductDb.Common.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Data.BiggBrandsDb
{
    public class User : EntityBase
    {
        [Required]
        [StringLength(750)]
        public string Username { get; set; }

        [Required]
        [StringLength(750)]
        public string Email { get; set; }

        public byte[] Password { get; set; }

        public int? UserRoleId { get; set; }

        [ForeignKey("UserRoleId")]
        public virtual UserRole UserRole { get; set; }
    }
}