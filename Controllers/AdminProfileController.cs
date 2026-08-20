using HostelManagementSystem.Data;
using HostelManagementSystem.Models;
using HostelManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HostelManagementSystem.Controllers;

[Authorize(Roles = "Admin")]
public class AdminProfileController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public AdminProfileController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ApplicationDbContext context,
        IWebHostEnvironment environment)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _environment = environment;
    }

    // ==========================================
    // PROFILE
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return RedirectToAction("Login", "Account");

        var profile = await _context.AdminProfiles
            .FirstOrDefaultAsync(x => x.IdentityUserId == user.Id);

        if (profile == null)
            return NotFound();

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
    // EDIT PROFILE
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return RedirectToAction("Login", "Account");

        var profile = await _context.AdminProfiles
            .FirstOrDefaultAsync(x => x.IdentityUserId == user.Id);

        if (profile == null)
            return NotFound();

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
            return RedirectToAction("Login", "Account");

        var profile = await _context.AdminProfiles
            .FirstOrDefaultAsync(x => x.IdentityUserId == identityUser.Id);

        if (profile == null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            model.CurrentProfilePicture = profile.ProfilePicture;
            return View(model);
        }

        // ==========================================
        // PASSWORD VALIDATION
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

            var passwordValid =
                await _userManager.CheckPasswordAsync(
                    identityUser,
                    model.CurrentPassword);

            if (!passwordValid)
            {
                ModelState.AddModelError(
                    "CurrentPassword",
                    "The current password is incorrect.");

                model.CurrentProfilePicture = profile.ProfilePicture;
                return View(model);
            }
        }

        // ==========================================
        // DUPLICATE EMAIL CHECK
        // ==========================================

        if (!string.Equals(
                identityUser.Email,
                model.Email,
                StringComparison.OrdinalIgnoreCase))
        {
            var existing =
                await _userManager.FindByEmailAsync(model.Email);

            if (existing != null &&
                existing.Id != identityUser.Id)
            {
                ModelState.AddModelError(
                    "Email",
                    "This email address is already in use.");

                model.CurrentProfilePicture = profile.ProfilePicture;
                return View(model);
            }
        }

        // ==========================================
        // UPDATE ADMIN PROFILE
        // ==========================================

        profile.FullName = model.FullName;
        profile.Department = model.Department;
        profile.PhoneNumber = model.PhoneNumber;
        profile.Address = model.Address;
        profile.UpdatedAt = DateTime.UtcNow;

        // ==========================================
// UPDATE IDENTITY EMAIL / USERNAME
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

    identityUser.EmailConfirmed = true;

    await _userManager.UpdateAsync(identityUser);
}
        // ==========================================
        // CHANGE PASSWORD
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

                model.CurrentProfilePicture = profile.ProfilePicture;
                return View(model);
            }
        }

        // ==========================================
        // PROFILE PICTURE
        // ==========================================

        if (model.ProfilePicture != null &&
            model.ProfilePicture.Length > 0)
        {
            var uploads =
                Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "admins");

            Directory.CreateDirectory(uploads);

            var extension =
                Path.GetExtension(model.ProfilePicture.FileName);

            var fileName =
                $"{identityUser.Id}{extension}";

            var filePath =
                Path.Combine(uploads, fileName);

            using var stream =
                new FileStream(filePath, FileMode.Create);

            await model.ProfilePicture.CopyToAsync(stream);

            profile.ProfilePicture =
                $"/uploads/admins/{fileName}";
        }

        // ==========================================
        // SAVE PROFILE
        // ==========================================

        _context.AdminProfiles.Update(profile);
        await _context.SaveChangesAsync();

        // Refresh the authentication cookie after email/password updates
        await _signInManager.RefreshSignInAsync(identityUser);

        TempData["Success"] =
            "Administrator profile updated successfully.";

        return RedirectToAction(nameof(Index));
    }
}