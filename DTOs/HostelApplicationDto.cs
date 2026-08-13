// DTOs/HostelApplicationDto.cs

namespace HostelManagementSystem.DTOs
{
    public class HostelApplicationDto
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int? HostelRoomId { get; set; }

        public DateTime ApplicationDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public StudentDto? Student { get; set; }

        public HostelRoomDto? HostelRoom { get; set; }

        public string StudentName =>
            Student?.FullName ?? string.Empty;

        public string StudentEmail =>
            Student?.Email ?? string.Empty;

        public string RoomNumber =>
            HostelRoom?.RoomNumber ?? string.Empty;

        public string HostelType =>
            HostelRoom?.HostelType ?? string.Empty;
    }
}