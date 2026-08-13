using HostelManagementSystem.Models;

namespace HostelManagementSystem.ViewModels
{
    public class AdminDashboardViewModel
    {
        // Statistics
        public int TotalStudents { get; set; }
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int AcceptedApplications { get; set; }
        public int RejectedApplications { get; set; }

        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; }
        public int FullRooms { get; set; }
        public int BoysRooms { get; set; }

        public int GirlsRooms { get; set; }

        public int TotalPayments { get; set; }
        public decimal TotalRevenue { get; set; }

        // Recent Data
        public List<HostelApplication> RecentApplications { get; set; } = new();

        public List<Payment> RecentPayments { get; set; } = new();

        public List<HostelRoom> RecentRooms { get; set; } = new();

        public int OccupiedBeds { get; set; }
        public int TotalBedSpace { get; set; }  


        public int UnreadMessages { get; set; }
        public List<ContactMessage> RecentMessages { get; set; } = new();

        public int PendingPayments { get; set; }
    }
}