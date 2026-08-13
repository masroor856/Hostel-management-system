// DTOs/CreateHostelApplicationDto.cs

using System.ComponentModel.DataAnnotations;

namespace HostelManagementSystem.DTOs
{
    public class CreateHostelApplicationDto
    {
        [Required]
        public int HostelRoomId { get; set; }
    }
}