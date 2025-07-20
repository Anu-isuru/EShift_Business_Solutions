using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }              // Maps to customer_id
        public string FirstName { get; set; }            // Maps to f_name
        public string LastName { get; set; }             // Maps to l_name
        public string Email { get; set; }                // Maps to email
        public string ContactNo { get; set; }            // Maps to contact_no
        public DateTime RegistrationDate { get; set; }   // Maps to registration_date
        public int UserId { get; set; }                  // Maps to fk_user_id
    }
}
