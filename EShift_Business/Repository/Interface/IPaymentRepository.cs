using EShift_Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Repository.Interface
{
    public interface IPaymentRepository
    {
        decimal CalculateTotalPriceByJobId(int jobId);
        bool InsertPayment(decimal paidAmount, DateTime paidDate, string method, string status, int jobId);
        List<Payment> GetPaymentsByCustomerId(int userId);
        int GetPendingPayments();
        Dictionary<string, float> GetMonthlyRevenue();
        Payment GetPaymentByJobId(int jobId);
        Dictionary<string, float> GetYearlyRevenue();



    }
}
