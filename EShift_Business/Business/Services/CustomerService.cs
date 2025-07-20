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
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public void Register(Customer customer)
        {
            _repository.RegisterCustomer(customer);
        }

        public bool EmailExists(string email)
        {
            return _repository.GetCustomerByEmail(email) != null;
        }
        public Customer GetCustomerByEmail(string email)
        {
            return _repository.GetCustomerByEmail(email);
        }
        public Customer GetCustomerByUserId(int userId)
        {
            return _repository.GetCustomerByUserId(userId);
        }
        public void UpdateCustomer(Customer customer)
        {
            _repository.UpdateCustomer(customer);
        }

    }
}
