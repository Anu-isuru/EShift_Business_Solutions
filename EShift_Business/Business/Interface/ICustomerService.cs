using EShift_Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Business.Interface
{
    public interface ICustomerService
    {
        void Register(Customer customer);
        bool EmailExists(string email);
        Customer GetCustomerByEmail(string email);
        Customer GetCustomerByUserId(int userId);
        void UpdateCustomer(Customer customer);

    }
}
