// DTOs/UpdatePaymentDto.cs

using System.ComponentModel.DataAnnotations;

namespace HostelManagementSystem.DTOs
{
    public class UpdatePaymentDto
    {
        public int Id { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Session { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = string.Empty;
    }
}