using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HostelManagementSystem.Models
{
    public class Payment
    {
        public int Id { get; set; }

        // Foreign key to HostelApplication
        public int HostelApplicationId { get; set; }

        [ForeignKey(nameof(HostelApplicationId))]
        public HostelApplication HostelApplication { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string Status { get; set; } = "Pending";

        public string? TransactionReference { get; set; }
        public string Session { get; set; } = string.Empty;
    }
}