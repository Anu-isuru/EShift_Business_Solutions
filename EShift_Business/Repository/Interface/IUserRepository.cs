using EShift_Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Repository.Interface
{
    public interface IUserRepository
    {
        int RegisterUser(User user);
        User GetUser(string email, string password, string role);
        int GetActiveCustomers();
    }
}
