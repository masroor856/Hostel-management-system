using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;

namespace HostelManagementSystem.Services
{
    public class ReportsService : IReportsService
    {
        private readonly IReportsRepository _repository;

        public ReportsService(
            IReportsRepository repository)
        {
            _repository = repository;
        }

        public async Task<ReportsDto> GetReportsAsync()
        {
            var recentPayments =
                await _repository.GetRecentPaymentsAsync(5);

            var recentApplications =
                await _repository.GetRecentApplicationsAsync(5);

            return new ReportsDto
            {
                TotalStudents =
                    await _repository.GetTotalStudentsAsync(),

                TotalRooms =
                    await _repository.GetTotalRoomsAsync(),

                TotalApplications =
                    await _repository.GetTotalApplicationsAsync(),

                TotalPayments =
                    await _repository.GetTotalPaymentsAsync(),

                TotalRevenue =
                    await _repository.GetTotalRevenueAsync(),

                AvailableRooms =
                    await _repository.GetAvailableRoomsAsync(),

                OccupiedRooms =
                    await _repository.GetOccupiedRoomsAsync(),

                RecentPayments =
                    recentPayments
                        .Select(p => new PaymentDto
                        {
                            Id = p.Id,

                            HostelApplicationId =
                                p.HostelApplicationId,

                            Amount =
                                p.Amount,

                            Session =
                                p.Session ?? string.Empty,

                            Status =
                                p.Status ?? string.Empty,

                            PaymentDate =
                                p.PaymentDate,

                            TransactionReference =
                                p.TransactionReference
                                ?? string.Empty,

                            StudentName =
                                p.HostelApplication?
                                    .Student?
                                    .FullName
                                ?? "Unknown Student",

                            StudentEmail =
                                p.HostelApplication?
                                    .Student?
                                    .Email
                                ?? string.Empty,

                            RoomNumber =
                                p.HostelApplication?
                                    .HostelRoom?
                                    .RoomNumber
                                ?? "Not Assigned"
                        })
                        .ToList(),

                RecentApplications =
                    recentApplications
                        .Select(a => new HostelApplicationDto
                        {
                            Id =
                                a.Id,

                            StudentId =
                                a.StudentId,

                            HostelRoomId =
                                a.HostelRoomId,

                            ApplicationDate =
                                a.ApplicationDate,

                            Status =
                                a.Status ?? string.Empty,

                            Student =
                                a.Student == null
                                    ? null
                                    : new StudentDto
                                    {
                                        Id =
                                            a.Student.Id,

                                        FullName =
                                            a.Student.FullName
                                            ?? "Unknown Student",

                                        Email =
                                            a.Student.Email
                                            ?? string.Empty,

                                        PhoneNumber =
                                            a.Student.PhoneNumber
                                            ?? string.Empty,

                                        Gender =
                                            a.Student.Gender
                                            ?? string.Empty,

                                        Department =
                                            a.Student.Department
                                            ?? string.Empty,

                                        ProfilePicture =
                                            a.Student.ProfilePicture
                                    },

                            HostelRoom =
                                a.HostelRoom == null
                                    ? null
                                    : new HostelRoomDto
                                    {
                                        Id =
                                            a.HostelRoom.Id,

                                        RoomNumber =
                                            a.HostelRoom.RoomNumber
                                            ?? "Not Assigned",

                                        HostelType =
                                            a.HostelRoom.HostelType
                                            ?? string.Empty,

                                        Capacity =
                                            a.HostelRoom.Capacity,

                                        AvailableSpace =
                                            a.HostelRoom.AvailableSpace,

                                        OccupiedSpace =
                                            a.HostelRoom.OccupiedSpace,

                                        IsAvailable =
                                            a.HostelRoom.IsAvailable,

                                        Price =
                                            a.HostelRoom.Price
                                    }
                        })
                        .ToList()
            };
        }
    }
}