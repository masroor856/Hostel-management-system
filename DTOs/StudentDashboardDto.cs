// DTOs/StudentDashboardDto.cs

using HostelManagementSystem.Models;

namespace HostelManagementSystem.DTOs;

public class StudentDashboardDto
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string? Gender { get; set; }

    public string? Department { get; set; }

    public string? ProfilePicture { get; set; }

    public ICollection<HostelApplication> HostelApplications { get; set; }
        = new List<HostelApplication>();

    public RoomAllocation? RoomAllocation { get; set; }

    public bool HasPaid =>
        HostelApplications
            .SelectMany(a => a.Payments ?? new List<Payment>())
            .Any(p => p.Status == "Paid");
}