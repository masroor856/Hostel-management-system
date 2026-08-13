// DTOs/AdminProfileDto.cs

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HostelManagementSystem.DTOs
{
    public class AdminProfileDto
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string? CurrentProfilePicture { get; set; }

        public IFormFile? ProfilePicture { get; set; }

        public string? CurrentPassword { get; set; }

        public string? NewPassword { get; set; }
    }
}