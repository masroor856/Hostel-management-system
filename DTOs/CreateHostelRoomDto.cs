using System.ComponentModel.DataAnnotations;

namespace HostelManagementSystem.DTOs
{
    public class CreateHostelRoomDto
    {
        [Required]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        public string HostelType { get; set; } = string.Empty;

        [Required]
        [Range(1, 10000000)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }
    }
}