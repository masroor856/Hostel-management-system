using System.ComponentModel.DataAnnotations;

namespace HostelManagementSystem.Models
{
    public class ResendConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}