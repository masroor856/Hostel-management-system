using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HostelManagementSystem.Models
{
    public class HostelRoom
    {
         public int Id { get; set; }

        [Required]
        public string RoomNumber { get; set; }
        public string HostelType { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        public int Capacity { get; set; }

        public int OccupiedSpace { get; set; }

        public bool IsAvailable { get; set; } = true;
        public int AvailableSpace { get; set; }
        public ICollection<RoomAllocation> RoomAllocations { get; set; }
         = new List<RoomAllocation>();
    }
}