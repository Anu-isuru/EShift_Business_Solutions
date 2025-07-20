using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Models
{
    public class Staff
    {
        public int StaffId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string LicenseNumber { get; set; }
        public string AvailabilityStatus { get; set; }
        public int UserId { get; set; } // FK from `user` table
        public DateTime CreatedDate { get; set; }

        public string Role { get; set; } // ✅ Add this line
    }
}
