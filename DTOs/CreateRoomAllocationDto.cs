// DTOs/CreateRoomAllocationDto.cs

using System.ComponentModel.DataAnnotations;

namespace HostelManagementSystem.DTOs
{
    public class CreateRoomAllocationDto
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int HostelRoomId { get; set; }
    }
}