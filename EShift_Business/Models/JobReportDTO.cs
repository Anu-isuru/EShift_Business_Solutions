using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Models
{
    public class JobReportDTO
    {
        public int JobId { get; set; }
        public string CustomerName { get; set; }
        public string PickupAddress { get; set; }
        public string DropoffAddress { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime DropoffDate { get; set; }
    }

}
