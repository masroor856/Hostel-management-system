using System.ComponentModel.DataAnnotations;

namespace HostelManagementSystem.DTOs;

public class UpdateHostelRoomDto
{
    public int Id { get; set; }

    [Required]
    public string RoomNumber { get; set; } = string.Empty;

    [Required]
    public string HostelType { get; set; } = string.Empty;

    [Required]
    [Range(1, 10000000)]
    public decimal Price { get; set; }

    [Required]
    [Range(1, 20)]
    public int Capacity { get; set; }
}