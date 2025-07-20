using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Models
{
    public class Load
    {
        public int LoadId { get; set; }
        public string Description { get; set; }
        public string Size { get; set; }
        public string Status { get; set; }
        public int JobId { get; set; }
        public List<int> ContainerIds { get; set; } = new List<int>();

    }
}
