using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Models
{
    public class Lorry
    {
        public int Id { get; set; }
        public string Registration_No { get; set; }
        public string Availability_State { get; set; } = "available";
        public string Condition { get; set; }
        public string Capacity { get; set; }
    }
}
