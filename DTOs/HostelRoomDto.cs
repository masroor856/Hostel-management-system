// DTOs/HostelRoomDto.cs

namespace HostelManagementSystem.DTOs
{
    public class HostelRoomDto
    {
        public int Id { get; set; }

        public string RoomNumber { get; set; } = string.Empty;

        public string HostelType { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public int AvailableSpace { get; set; }

        public int OccupiedSpace { get; set; }

        public bool IsAvailable { get; set; }

        public decimal Price { get; set; }
    }
}