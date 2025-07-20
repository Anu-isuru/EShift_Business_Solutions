using EShift_Business.Business.Interface;
using EShift_Business.Models;
using EShift_Business.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Business.Services
{
        public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        //public decimal CalculateTotalPriceByJobId(int jobId)
        //{
        //    throw new NotImplementedException();
        //}

        public List<Payment> GetPaymentsByCustomerId(int userId)
        {
            return _paymentRepository.GetPaymentsByCustomerId(userId);
        }
        public bool InsertPayment(decimal paidAmount, DateTime paidDate, string paymentMethod, string status, int jobId)
        {
            return _paymentRepository.InsertPayment(paidAmount, paidDate, paymentMethod, status, jobId);
        }
        public decimal GetTotalPaymentAmount(int jobId)
        {
            return _paymentRepository.CalculateTotalPriceByJobId(jobId);
        }
        public int GetPendingPayments()
        {
            return _paymentRepository.GetPendingPayments();
        }
        public Dictionary<string, float> GetMonthlyRevenue()
        {
            return _paymentRepository.GetMonthlyRevenue();
        }
        public Payment GetPaymentByJobId(int jobId)
        {
            return _paymentRepository.GetPaymentByJobId(jobId);
        }




    }
}
