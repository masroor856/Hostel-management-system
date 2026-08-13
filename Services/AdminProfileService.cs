// Services/AdminProfileService.cs

using HostelManagementSystem.DTOs;
using HostelManagementSystem.Interfaces;
using HostelManagementSystem.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

namespace HostelManagementSystem.Services
{
    public class AdminProfileService
        : IAdminProfileService
    {
        private readonly IAdminProfileRepository
            _repository;

        private readonly UserManager<IdentityUser>
            _userManager;

        private readonly IWebHostEnvironment
            _environment;

        public AdminProfileService(
            IAdminProfileRepository repository,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment environment)
        {
            _repository = repository;
            _userManager = userManager;
            _environment = environment;
        }

        public async Task<AdminProfileDto?>
            GetProfileAsync(
                string identityUserId,
                string email)
        {
            var profile =
                await _repository
                    .GetByIdentityUserIdAsync(
                        identityUserId);

            if (profile == null)
                return null;

            return Map(profile, email);
        }

        public async Task<AdminProfileDto?>
            GetEditProfileAsync(
                string identityUserId,
                string email)
        {
            var profile =
                await _repository
                    .GetByIdentityUserIdAsync(
                        identityUserId);

            if (profile == null)
                return null;

            return Map(profile, email);
        }

        public async Task<(
            bool Success,
            IEnumerable<string> Errors)>
            UpdateProfileAsync(
                string identityUserId,
                AdminProfileDto model)
        {
            var errors = new List<string>();

            var identityUser =
                await _userManager
                    .FindByIdAsync(identityUserId);

            if (identityUser == null)
            {
                errors.Add("Administrator account not found.");
                return (false, errors);
            }

            var profile =
                await _repository
                    .GetByIdentityUserIdAsync(
                        identityUserId);

            if (profile == null)
            {
                errors.Add("Administrator profile not found.");
                return (false, errors);
            }

            // ==========================
            // PASSWORD VALIDATION
            // ==========================

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(
                    model.CurrentPassword))
                {
                    errors.Add(
                        "Please enter your current password.");

                    return (false, errors);
                }

                var passwordValid =
                    await _userManager.CheckPasswordAsync(
                        identityUser,
                        model.CurrentPassword);

                if (!passwordValid)
                {
                    errors.Add(
                        "The current password is incorrect.");

                    return (false, errors);
                }
            }

            // ==========================
            // EMAIL VALIDATION
            // ==========================

            if (!string.Equals(
                identityUser.Email,
                model.Email,
                StringComparison.OrdinalIgnoreCase))
            {
                var existingUser =
                    await _userManager
                        .FindByEmailAsync(model.Email);

                if (existingUser != null &&
                    existingUser.Id != identityUser.Id)
                {
                    errors.Add(
                        "This email address is already being used.");

                    return (false, errors);
                }
            }

            // ==========================
            // UPDATE PROFILE
            // ==========================

            profile.FullName = model.FullName;
            profile.Department = model.Department;
            profile.PhoneNumber = model.PhoneNumber;
            profile.Address = model.Address;
            profile.UpdatedAt = DateTime.UtcNow;

            // ==========================
            // UPDATE EMAIL
            // ==========================

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
                    errors.AddRange(
                        emailResult.Errors
                            .Select(e => e.Description));

                    return (false, errors);
                }

                identityUser.EmailConfirmed = true;

                var usernameResult =
                    await _userManager.SetUserNameAsync(
                        identityUser,
                        model.Email);

                if (!usernameResult.Succeeded)
                {
                    errors.AddRange(
                        usernameResult.Errors
                            .Select(e => e.Description));

                    return (false, errors);
                }

                var updateResult =
                    await _userManager.UpdateAsync(
                        identityUser);

                if (!updateResult.Succeeded)
                {
                    errors.AddRange(
                        updateResult.Errors
                            .Select(e => e.Description));

                    return (false, errors);
                }
            }

            // ==========================
            // UPDATE PASSWORD
            // ==========================

            if (!string.IsNullOrWhiteSpace(
                model.NewPassword))
            {
                var passwordResult =
                    await _userManager.ChangePasswordAsync(
                        identityUser,
                        model.CurrentPassword!,
                        model.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    errors.AddRange(
                        passwordResult.Errors
                            .Select(e => e.Description));

                    return (false, errors);
                }
            }

            // ==========================
            // PROFILE PICTURE
            // ==========================

            if (model.ProfilePicture != null &&
                model.ProfilePicture.Length > 0)
            {
                var allowedExtensions =
                    new[] { ".jpg", ".jpeg", ".png", ".webp" };

                var extension =
                    Path.GetExtension(
                        model.ProfilePicture.FileName)
                        .ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    errors.Add(
                        "Only JPG, JPEG, PNG and WEBP images are allowed.");

                    return (false, errors);
                }

                if (model.ProfilePicture.Length >
                    2 * 1024 * 1024)
                {
                    errors.Add(
                        "Profile picture must not exceed 2 MB.");

                    return (false, errors);
                }

                var uploadsFolder =
                    Path.Combine(
                        _environment.WebRootPath,
                        "uploads",
                        "admins");

                Directory.CreateDirectory(
                    uploadsFolder);

                // Delete old picture
                if (!string.IsNullOrWhiteSpace(
                    profile.ProfilePicture))
                {
                    var oldPath =
                        Path.Combine(
                            _environment.WebRootPath,
                            profile.ProfilePicture
                                .TrimStart('/')
                                .Replace(
                                    "/",
                                    Path.DirectorySeparatorChar
                                        .ToString()));

                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                var fileName =
                    $"{identityUser.Id}{extension}";

                var filePath =
                    Path.Combine(
                        uploadsFolder,
                        fileName);

                await using var stream =
                    new FileStream(
                        filePath,
                        FileMode.Create);

                await model.ProfilePicture
                    .CopyToAsync(stream);

                profile.ProfilePicture =
                    $"/uploads/admins/{fileName}";
            }

            // ==========================
            // SAVE
            // ==========================

            await _repository.UpdateAsync(profile);

            await _repository.SaveChangesAsync();

            return (true, errors);
        }

        private static AdminProfileDto Map(
            AdminProfile profile,
            string email)
        {
            return new AdminProfileDto
            {
                Id = profile.Id,
                FullName = profile.FullName,
                Email = email,
                Department = profile.Department,
                PhoneNumber = profile.PhoneNumber,
                Address = profile.Address,
                CurrentProfilePicture =
                    profile.ProfilePicture
            };
        }
    }
}