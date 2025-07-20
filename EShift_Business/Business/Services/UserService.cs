using EShift_Business.Business.Interface;
using EShift_Business.Models;
using EShift_Business.Repository.Interface;
using EShift_Business.Repository.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        // You can inject it, or create new directly
        public UserService()
        {
            _userRepository = new UserRepository();
        }

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public int RegisterUser(User user)
        {
            return _userRepository.RegisterUser(user);
        }

        public User GetUser(string email, string password, string role)
        {
            return _userRepository.GetUser(email, password, role);
        }
        public int GetActiveCustomers()
        {
            return _userRepository.GetActiveCustomers();
        }

    }
}


