// DTOs/CreateStudentDto.cs
namespace HostelManagementSystem.DTOs
{
    public class CreateStudentDto
    {
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Gender { get; set; } = "";
        public string Department { get; set; } = "";
    }
}