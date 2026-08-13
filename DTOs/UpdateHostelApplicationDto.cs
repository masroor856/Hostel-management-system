// DTOs/UpdateHostelApplicationDto.cs

using System.ComponentModel.DataAnnotations;

namespace HostelManagementSystem.DTOs
{
    public class UpdateHostelApplicationDto
    {
        public int Id { get; set; }

        [Required]
        public int HostelRoomId { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;
    }
}