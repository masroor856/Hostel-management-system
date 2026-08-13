// DTOs/EditStudentProfileDto.cs

using System.ComponentModel.DataAnnotations;

namespace HostelManagementSystem.DTOs
{
    public class EditStudentProfileDto
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public string Department { get; set; } = string.Empty;
    }
}