// DTOs/ReportsDto.cs

namespace HostelManagementSystem.DTOs
{
    public class ReportsDto
    {
        public int TotalStudents { get; set; }

        public int TotalRooms { get; set; }

        public int TotalApplications { get; set; }

        public int TotalPayments { get; set; }

        public decimal TotalRevenue { get; set; }

        public int AvailableRooms { get; set; }

        public int OccupiedRooms { get; set; }

        public IEnumerable<PaymentDto> RecentPayments { get; set; }
            = new List<PaymentDto>();

        public IEnumerable<HostelApplicationDto> RecentApplications
            { get; set; }
                = new List<HostelApplicationDto>();
    }
}