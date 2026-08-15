// Controllers/StudentController.cs

using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        public async Task<IActionResult> Index(
            string? search,
            int page = 1)
        {
            const int pageSize = 10;

            if (page < 1)
                page = 1;

            var students = await _studentService.SearchAsync(search);

            var totalStudents = students.Count();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages =
                (int)Math.Ceiling(
                    totalStudents / (double)pageSize);

            ViewBag.Search = search;

            var data = students
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToModel)
                .ToList();

            return View(data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var dto =
                await _studentService.GetByIdAsync(id);

            if (dto == null)
                return NotFound();

            return View(MapToModel(dto));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (!ModelState.IsValid)
                return View(student);

            var dto = new CreateStudentDto
            {
                FullName = student.FullName,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                Gender = student.Gender,
                Department = student.Department
            };

            await _studentService.CreateAsync(dto);

            TempData["Success"] =
                "Student created successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto =
                await _studentService.GetByIdAsync(id);

            if (dto == null)
                return NotFound();

            return View(MapToModel(dto));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Student student)
        {
            if (id != student.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(student);

            var dto = new UpdateStudentDto
            {
                Id = student.Id,
                FullName = student.FullName,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                Gender = student.Gender,
                Department = student.Department
            };

            var updated =
                await _studentService.UpdateAsync(dto);

            if (!updated)
                return NotFound();

            TempData["Success"] =
                "Student updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dto =
                await _studentService.GetByIdAsync(id);

            if (dto == null)
                return NotFound();

            return View(MapToModel(dto));
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted =
                await _studentService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            TempData["Success"] =
                "Student deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private static Student MapToModel(StudentDto dto)
        {
            return new Student
            {
                Id = dto.Id,
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Gender = dto.Gender,
                Department = dto.Department,
            };
        }
    }
}