// DTOs/PaymentDto.cs

namespace HostelManagementSystem.DTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }

        public int HostelApplicationId { get; set; }

        public decimal Amount { get; set; }

        public string Session { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime PaymentDate { get; set; }

        public string TransactionReference { get; set; } = string.Empty;

        public string StudentName { get; set; } = string.Empty;

        public string StudentEmail { get; set; } = string.Empty;

        public string RoomNumber { get; set; } = string.Empty;
    }
}