// DTOs/CreatePaymentDto.cs

using System.ComponentModel.DataAnnotations;

namespace HostelManagementSystem.DTOs
{
    public class CreatePaymentDto
    {
        [Required]
        public int HostelApplicationId { get; set; }

        [Required]
        public string Session { get; set; } = string.Empty;
    }
}