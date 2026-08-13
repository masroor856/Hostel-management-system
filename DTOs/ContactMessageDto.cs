// DTOs/ContactMessageDto.cs

using System.ComponentModel.DataAnnotations;

namespace HostelManagementSystem.DTOs
{
    public class ContactMessageDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;
    }
}