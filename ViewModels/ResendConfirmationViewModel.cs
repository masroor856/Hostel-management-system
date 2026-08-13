using System.ComponentModel.DataAnnotations;

namespace HostelManagementSystem.ViewModels
{
    public class ResendConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}