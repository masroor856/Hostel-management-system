using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HostelManagementSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace HostelManagementSystem.Controllers;

[Authorize]
public class StudentDashboardController : Controller
{
    private readonly IStudentDashboardService _studentDashboardService;

    public StudentDashboardController(
        IStudentDashboardService studentDashboardService)
    {
        _studentDashboardService = studentDashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var email = User.Identity?.Name;

        if (string.IsNullOrEmpty(email))
            return Challenge();

        var dto =
            await _studentDashboardService.GetDashboardAsync(email);

        if (dto == null)
        {
            TempData["Error"] = "Student record not found.";
            return RedirectToAction("Index", "Home");
        }

        return View(dto);
    }

    [HttpGet]
public async Task<IActionResult> Profile()
{
    var email = User.Identity?.Name;

    if (string.IsNullOrEmpty(email))
        return Challenge();

    var model =
        await _studentDashboardService.GetProfileAsync(email);

    if (model == null)
    {
        TempData["Error"] = "Student profile not found.";
        return RedirectToAction(nameof(Index));
    }

    return View(model);
}

    [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Profile(Student student)
{
    if (!ModelState.IsValid)
        return View(student);

    var email = User.Identity?.Name;

    if (string.IsNullOrEmpty(email))
        return Challenge();

    var dto = new EditProfileViewModel
    {
        Id = student.Id,
        FullName = student.FullName,
        Email = student.Email,
        PhoneNumber = student.PhoneNumber,
        Gender = student.Gender,
        Department = student.Department,
        ProfilePicture = student.ProfilePicture
    };

    var updated = await _studentDashboardService.UpdateProfileAsync(email, dto);

    if (!updated)
    {
        TempData["Error"] = "Unable to update profile.";
        return View(student);
    }

    TempData["Success"] = "Profile updated successfully.";
    return RedirectToAction(nameof(Profile));
}

[HttpGet]
public async Task<IActionResult> EditProfile()
{
    var email = User.Identity?.Name;

    if (string.IsNullOrEmpty(email))
        return Challenge();

    var dto = await _studentDashboardService.GetProfileAsync(email);

    if (dto == null)
    {
        TempData["Error"] = "Student profile not found.";
        return RedirectToAction(nameof(Index));
    }

    var student = new Student
    {
        Id = dto.Id,
        FullName = dto.FullName,
        Email = dto.Email,
        PhoneNumber = dto.PhoneNumber,
        Gender = dto.Gender,
        Department = dto.Department,
        ProfilePicture = dto.ProfilePicture
    };

    return View(student);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EditProfile(
    [Bind("Id,FullName,Email,PhoneNumber,Gender,Department,ProfilePicture")]
    Student student)
{
    var email = User.Identity?.Name;

    if (string.IsNullOrEmpty(email))
        return Challenge();

    // Always use the logged-in user's email
    student.Email = email;

    // Remove validation for fields not posted
    ModelState.Remove(nameof(Student.Email));
    ModelState.Remove(nameof(Student.HostelApplications));
    ModelState.Remove(nameof(Student.RoomAllocation));

    if (!ModelState.IsValid)
        return View(student);

    var dto = new EditProfileViewModel
    {
        Id = student.Id,
        FullName = student.FullName,
        Email = student.Email,
        PhoneNumber = student.PhoneNumber,
        Gender = student.Gender,
        Department = student.Department,
        ProfilePicture = student.ProfilePicture
    };

    var updated = await _studentDashboardService.UpdateProfileAsync(email, dto);

    if (!updated)
    {
        TempData["Error"] = "Unable to update profile.";
        return View(student);
    }

    TempData["Success"] = "Profile updated successfully.";

    return RedirectToAction(nameof(Profile));
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
{
    var email = User.Identity?.Name;

    if (string.IsNullOrEmpty(email))
        return Challenge();

    if (profilePicture == null || profilePicture.Length == 0)
    {
        TempData["Error"] = "Please select an image.";
        return RedirectToAction(nameof(EditProfile));
    }

    var uploadsFolder = Path.Combine(
        Directory.GetCurrentDirectory(),
        "wwwroot",
        "uploads",
        "profiles");

    Directory.CreateDirectory(uploadsFolder);

    var fileName =
        $"{Guid.NewGuid()}{Path.GetExtension(profilePicture.FileName)}";

    var filePath = Path.Combine(uploadsFolder, fileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await profilePicture.CopyToAsync(stream);
    }

    var profile = await _studentDashboardService.GetProfileAsync(email);

    if (profile == null)
    {
        TempData["Error"] = "Student profile not found.";
        return RedirectToAction(nameof(EditProfile));
    }

    profile.ProfilePicture = $"/uploads/profiles/{fileName}";

    await _studentDashboardService.UpdateProfileAsync(email, profile);

    TempData["Success"] = "Profile picture updated successfully.";

    return RedirectToAction(nameof(Profile));
}

}