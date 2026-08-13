using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HostelManagementSystem.Models
{
    public class HostelApplication
    {
         public int Id { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [ForeignKey("HostelRoom")]
        public int HostelRoomId { get; set; }

        public DateTime ApplicationDate { get; set; }

        public string Status { get; set; }

        public Student Student { get; set; }

        public HostelRoom HostelRoom { get; set; }

    
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}