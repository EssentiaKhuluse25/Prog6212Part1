using System.ComponentModel.DataAnnotations;

namespace Prog6212Part1.Models
{
    public class Roles
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string RoleName { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
    }
}

