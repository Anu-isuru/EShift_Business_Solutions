using EShift_Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Business.Interface
{
    public interface IUserService
    {
        int RegisterUser(User user);                 // For registration
        User GetUser(string email, string password, string role);  // For login
        int GetActiveCustomers();

    }
}
