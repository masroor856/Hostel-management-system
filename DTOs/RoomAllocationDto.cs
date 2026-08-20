namespace HostelManagementSystem.DTOs
{
    public class RoomAllocationDto
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int HostelRoomId { get; set; }

        public DateTime AllocationDate { get; set; }

        public bool IsActive { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string StudentEmail { get; set; } = string.Empty;

        public string RoomNumber { get; set; } = string.Empty;

        public string HostelType { get; set; } = string.Empty;

        public HostelRoomDto? HostelRoom { get; set; }
    }
}