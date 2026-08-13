using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMS.ViewModels
{
   public class EditProfileViewModel
{
    public int Id { get; set; }

    public string Email { get; set; }
    public string FullName { get; set; }

    public string PhoneNumber { get; set; }

    public string Gender { get; set; }
    public string Department { get; set; } = string.Empty;
}
}