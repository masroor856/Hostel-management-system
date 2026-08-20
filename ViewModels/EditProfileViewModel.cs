using HostelManagementSystem.DTOs;

namespace HostelManagementSystem.ViewModels
{
    public class EditProfileViewModel
    {
        public int Id { get; set; }

        public string? ProfilePicture { get; set; }

        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        // Hostel information
        public string? ApplicationStatus { get; set; }

        public string? PaymentStatus { get; set; }

        public string? HostelType { get; set; }

        public string? RoomNumber { get; set; }

        public bool HasAllocation { get; set; }

        public DateTime? ApplicationDate { get; set; }

        public string? PaymentSession { get; set; }

        public List<HostelApplicationDto> HostelApplications { get; set; }
            = new();

        public RoomAllocationDto? RoomAllocation { get; set; }
    }
}