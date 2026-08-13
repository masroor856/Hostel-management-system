using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HostelManagementSystem.ViewModels
{
    public class AdminProfileViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        public string? Department { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? CurrentProfilePicture { get; set; }

        public IFormFile? ProfilePicture { get; set; }

        // Password
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare(
            "NewPassword",
            ErrorMessage = "The passwords do not match."
        )]
        [Display(Name = "Confirm New Password")]
        public string? ConfirmPassword { get; set; }
    }
}