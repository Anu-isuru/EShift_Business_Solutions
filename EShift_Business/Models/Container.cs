using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Models
{
    public class Container
    {
        public int ContainerId { get; set; }
        public string Size { get; set; }
        public float Price { get; set; }
        public string Status { get; set; }
        public string Condition { get; set; }
    }
}
