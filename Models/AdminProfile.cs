using System.ComponentModel.DataAnnotations;

namespace HostelManagementSystem.Models
{
    public class AdminProfile
    {
        public int Id { get; set; }

        // ASP.NET Identity User Id
        [Required]
        public string IdentityUserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Email { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? Department { get; set; }

        [StringLength(500)]
        public string? ProfilePicture { get; set; }

        [StringLength(50)]
        public string? PhoneNumber { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}