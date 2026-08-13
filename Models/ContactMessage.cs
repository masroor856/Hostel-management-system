using System;
using System.ComponentModel.DataAnnotations;

namespace HostelManagementSystem.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime DateSent { get; set; } = DateTime.Now;

        public bool IsRead { get; set; } = false;
    }
}