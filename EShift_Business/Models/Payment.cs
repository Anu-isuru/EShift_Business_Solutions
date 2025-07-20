using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime PaidDate { get; set; }
        public String PaymentMethod {  get; set; }
        public string Status { get; set; }
        public int JobId { get; set; }

    }
}
