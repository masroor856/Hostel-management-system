using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HostelManagementSystem.Models
{
    public class Student
    {
          public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        public string Gender { get; set; }

        

        public string Department { get; set; } = string.Empty;

        public string Level { get; set; } = string.Empty;



        public ICollection<HostelApplication> HostelApplications { get; set; }

        public virtual RoomAllocation RoomAllocation { get; set; }

        public string? ProfilePicture { get; set; }

        public DateTime? ProfilePictureUpdated { get; set; }
    }
}