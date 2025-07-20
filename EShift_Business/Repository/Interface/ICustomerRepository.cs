using EShift_Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Repository.Interface
{
    public interface ICustomerRepository
    {
        void RegisterCustomer(Customer customer);
        Customer GetCustomerByEmail(string email);
        Customer GetCustomerByUserId(int userId);
        void UpdateCustomer(Customer customer);
    }
}
