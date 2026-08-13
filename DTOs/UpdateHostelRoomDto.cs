// DTOs/UpdateHostelRoomDto.cs
namespace HostelManagementSystem.DTOs
{
    public class UpdateHostelRoomDto
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string HostelType { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
}