using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Models
{
    public class Job
    {
        public int JobId { get; set; }
        public int PickupLocationId { get; set; }
        public int DropoffLocationId { get; set; }
        public int UserId { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime DropoffDate { get; set; }
        public string Status { get; set; } = "Pending";

        // Pickup
        public string PickupAddress { get; set; }
        public string PickupCity { get; set; }
        public string PickupProvince { get; set; }

        // Dropoff
        public string DropoffAddress { get; set; }
        public string DropoffCity { get; set; }
        public string DropoffProvince { get; set; }
    }

}
