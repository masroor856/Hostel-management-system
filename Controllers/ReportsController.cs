using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly IReportsService _reportsService;

        public ReportsController(
            IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        public async Task<IActionResult> Index()
        {
            var report =
                await _reportsService.GetReportsAsync();

            ViewBag.TotalStudents =
                report.TotalStudents;

            ViewBag.TotalRooms =
                report.TotalRooms;

            ViewBag.TotalApplications =
                report.TotalApplications;

            ViewBag.TotalPayments =
                report.TotalPayments;

            ViewBag.TotalRevenue =
                report.TotalRevenue;

            ViewBag.AvailableRooms =
                report.AvailableRooms;

            ViewBag.OccupiedRooms =
                report.OccupiedRooms;

            /*
             * The existing Reports view expects
             * Payment.HostelApplication.Student.
             *
             * Therefore we rebuild Payment models here
             * instead of sending PaymentDto objects.
             */
            ViewBag.RecentPayments =
                report.RecentPayments
                    .Select(p => new Payment
                    {
                        Id = p.Id,

                        HostelApplicationId =
                            p.HostelApplicationId,

                        Amount =
                            p.Amount,

                        Session =
                            p.Session,

                        Status =
                            p.Status,

                        PaymentDate =
                            p.PaymentDate,

                        TransactionReference =
                            p.TransactionReference,

                        HostelApplication =
                            new HostelApplication
                            {
                                Id =
                                    p.HostelApplicationId,

                                Student =
                                    new Student
                                    {
                                        FullName =
                                            p.StudentName,

                                        Email =
                                            p.StudentEmail
                                    },

                                HostelRoom =
                                    new HostelRoom
                                    {
                                        RoomNumber =
                                            p.RoomNumber
                                    }
                            }
                    })
                    .ToList();

          ViewBag.RecentApplications =
    report.RecentApplications
        .Select(a => new HostelApplication
        {
            Id = a.Id,

            StudentId = a.StudentId,

            HostelRoomId =
                a.HostelRoomId ?? 0,

            ApplicationDate =
                a.ApplicationDate,

            Status =
                a.Status ?? string.Empty,

            Student =
                a.Student == null
                    ? new Student
                    {
                        Id = a.StudentId,
                        FullName = "Unknown Student",
                        Email = string.Empty
                    }
                    : new Student
                    {
                        Id = a.Student.Id,
                        FullName =
                            a.Student.FullName ?? "Unknown Student",
                        Email =
                            a.Student.Email ?? string.Empty
                    },

            HostelRoom =
                a.HostelRoom == null
                    ? new HostelRoom
                    {
                        Id = a.HostelRoomId ?? 0,
                        RoomNumber = "Not Assigned"
                    }
                    : new HostelRoom
                    {
                        Id = a.HostelRoom.Id,
                        
                        RoomNumber =
                            a.HostelRoom.RoomNumber ?? "Not Assigned",
                        HostelType =
                            a.HostelRoom.HostelType ?? string.Empty,
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
        .ToList();
            return View("Index");
        }
    }
}