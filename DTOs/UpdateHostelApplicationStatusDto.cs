// DTOs/UpdateApplicationStatusDto.cs

using System.ComponentModel.DataAnnotations;

namespace HostelManagementSystem.DTOs
{
    public class UpdateApplicationStatusDto
    {
        public int Id { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;
    }
}