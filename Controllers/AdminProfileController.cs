using HostelManagementSystem.Data;
using HostelManagementSystem.Models;
using HostelManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HostelManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
public class AdminProfileController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public AdminProfileController(
        UserManager<IdentityUser> userManager,
        ApplicationDbContext context,
        IWebHostEnvironment environment)
    {
        _userManager = userManager;
        _context = context;
        _environment = environment;
    }

    [HttpGet]
public async Task<IActionResult> Index()
{
    var user = await _userManager.GetUserAsync(User);

    if (user == null)
    {
        return RedirectToAction("Login", "Account");
    }

    var profile = await _context.AdminProfiles
        .FirstOrDefaultAsync(x => x.IdentityUserId == user.Id);

    if (profile == null)
    {
        return NotFound();
    }

   var model = new AdminProfileViewModel
{
    Id = profile.Id,
    FullName = profile.FullName,
    Email = user.Email ?? "",
    Department = profile.Department,
    PhoneNumber = profile.PhoneNumber,
    Address = profile.Address,
    CurrentProfilePicture = profile.ProfilePicture
};

return View(model);
}

    [HttpGet]
public async Task<IActionResult> Edit()
{
    var user = await _userManager.GetUserAsync(User);

    if (user == null)
    {
        return RedirectToAction("Login", "Account");
    }

    var profile = await _context.AdminProfiles
        .FirstOrDefaultAsync(x => x.IdentityUserId == user.Id);

    if (profile == null)
    {
        return NotFound();
    }

    var model = new AdminProfileViewModel
{
    Id = profile.Id,
    FullName = profile.FullName,
    Email = user.Email ?? "",
    Department = profile.Department,
    PhoneNumber = profile.PhoneNumber,
    Address = profile.Address,
    CurrentProfilePicture = profile.ProfilePicture
};

return View(model);
}

        // ==========================================
        // UPDATE PROFILE
        // ==========================================

       [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(AdminProfileViewModel model)
{
    var identityUser = await _userManager.GetUserAsync(User);

    if (identityUser == null)
    {
        return RedirectToAction("Login", "Account");
    }

    var profile = await _context.AdminProfiles
        .FirstOrDefaultAsync(x => x.IdentityUserId == identityUser.Id);

    if (profile == null)
    {
        return NotFound();
    }

    // ==========================================
    // VALIDATION
    // ==========================================

    if (!ModelState.IsValid)
    {
        model.CurrentProfilePicture = profile.ProfilePicture;
        return View(model);
    }


    // ==========================================
    // 1. VALIDATE CURRENT PASSWORD FIRST
    // ==========================================

    if (!string.IsNullOrWhiteSpace(model.NewPassword))
    {
        if (string.IsNullOrWhiteSpace(model.CurrentPassword))
        {
            ModelState.AddModelError(
                "CurrentPassword",
                "Please enter your current password.");

            model.CurrentProfilePicture = profile.ProfilePicture;

            return View(model);
        }

        var passwordCheck =
            await _userManager.CheckPasswordAsync(
                identityUser,
                model.CurrentPassword);

        if (!passwordCheck)
        {
            ModelState.AddModelError(
                "CurrentPassword",
                "The current password is incorrect.");

            model.CurrentProfilePicture = profile.ProfilePicture;

            return View(model);
        }
    }


    // ==========================================
    // 2. CHECK NEW EMAIL BEFORE MAKING CHANGES
    // ==========================================

    if (!string.Equals(
            identityUser.Email,
            model.Email,
            StringComparison.OrdinalIgnoreCase))
    {
        var existingUser =
            await _userManager.FindByEmailAsync(model.Email);

        if (existingUser != null &&
            existingUser.Id != identityUser.Id)
        {
            ModelState.AddModelError(
                "Email",
                "This email address is already being used.");

            model.CurrentProfilePicture =
                profile.ProfilePicture;

            return View(model);
        }
    }


    // ==========================================
    // UPDATE PROFILE INFORMATION
    // ==========================================

    profile.FullName = model.FullName;

    profile.Department = model.Department;

    profile.PhoneNumber = model.PhoneNumber;

    profile.Address = model.Address;

    profile.UpdatedAt = DateTime.UtcNow;


    // ==========================================
    // UPDATE EMAIL
    // ==========================================

    if (!string.Equals(
            identityUser.Email,
            model.Email,
            StringComparison.OrdinalIgnoreCase))
    {
        var emailResult =
            await _userManager.SetEmailAsync(
                identityUser,
                model.Email);

        if (!emailResult.Succeeded)
        {
            foreach (var error in emailResult.Errors)
            {
                ModelState.AddModelError(
                    "Email",
                    error.Description);
            }

            model.CurrentProfilePicture =
                profile.ProfilePicture;

            return View(model);
        }


        // New admin email is immediately confirmed
        identityUser.EmailConfirmed = true;


        // Keep username synchronized with email
        var usernameResult =
            await _userManager.SetUserNameAsync(
                identityUser,
                model.Email);

        if (!usernameResult.Succeeded)
        {
            foreach (var error in usernameResult.Errors)
            {
                ModelState.AddModelError(
                    "Email",
                    error.Description);
            }

            model.CurrentProfilePicture =
                profile.ProfilePicture;

            return View(model);
        }


        // Save Identity email changes
        var identityUpdateResult =
            await _userManager.UpdateAsync(identityUser);

        if (!identityUpdateResult.Succeeded)
        {
            foreach (var error in identityUpdateResult.Errors)
            {
                ModelState.AddModelError(
                    "Email",
                    error.Description);
            }

            model.CurrentProfilePicture =
                profile.ProfilePicture;

            return View(model);
        }
    }


    // ==========================================
    // 8. UPDATE PASSWORD
    // ==========================================

    if (!string.IsNullOrWhiteSpace(model.NewPassword))
    {
        var passwordResult =
            await _userManager.ChangePasswordAsync(
                identityUser,
                model.CurrentPassword!,
                model.NewPassword);

        if (!passwordResult.Succeeded)
        {
            foreach (var error in passwordResult.Errors)
            {
                ModelState.AddModelError(
                    "NewPassword",
                    error.Description);
            }

            model.CurrentProfilePicture =
                profile.ProfilePicture;

            return View(model);
        }
    }


    // ==========================================
    // UPDATE PROFILE PICTURE
    // ==========================================

    if (model.ProfilePicture != null &&
        model.ProfilePicture.Length > 0)
    {
        var uploadsFolder =
            Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "admins");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var extension =
            Path.GetExtension(
                model.ProfilePicture.FileName);

        var fileName =
            $"{identityUser.Id}{extension}";

        var filePath =
            Path.Combine(
                uploadsFolder,
                fileName);

        using (var stream =
               new FileStream(
                   filePath,
                   FileMode.Create))
        {
            await model.ProfilePicture.CopyToAsync(stream);
        }

        profile.ProfilePicture =
            $"/uploads/admins/{fileName}";
    }


    // ==========================================
    // SAVE PROFILE
    // ==========================================

    _context.AdminProfiles.Update(profile);

    await _context.SaveChangesAsync();


    // ==========================================
    // SUCCESS
    // ==========================================

    TempData["Success"] =
        "Administrator profile updated successfully.";

    return RedirectToAction(nameof(Index));
}
}
}