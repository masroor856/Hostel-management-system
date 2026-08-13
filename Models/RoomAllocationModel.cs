using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HostelManagementSystem.Models
{
    public class RoomAllocation
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int HostelRoomId { get; set; }

        public DateTime AllocationDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        [ForeignKey("HostelRoomId")]
        public virtual HostelRoom HostelRoom { get; set; }
    }
}