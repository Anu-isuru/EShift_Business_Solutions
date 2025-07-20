using EShift_Business.Models;
using EShift_Business.Repository.Interface;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Repository.Services
{
    public class PaymentRepository : IPaymentRepository
    {
        private string connectionString = "Server=localhost;Database=eshift_db;Uid=root;Pwd=1234;";
        public decimal CalculateTotalPriceByJobId(int jobId)
        {

            decimal total = 0;
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT SUM(c.price)
            FROM `load` l
            JOIN load_container lc ON l.load_id = lc.load_id
            JOIN container c ON lc.container_id = c.container_id
            WHERE l.fk_job_id = @jobId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@jobId", jobId);

                    var result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        total = Convert.ToDecimal(result);
                    }
                }
            }
            return total;
        }
        public bool InsertPayment(decimal paidAmount, DateTime paidDate, string method, string status, int jobId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO payment (paid_amount, paid_date, payment_method, status, fk_job_id)
                         VALUES (@amount, @date, @method, @status, @jobId)";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@amount", paidAmount);
                    cmd.Parameters.AddWithValue("@date", paidDate);
                    cmd.Parameters.AddWithValue("@method", method);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@jobId", jobId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public List<Payment> GetPaymentsByCustomerId(int userId)
        {
            List<Payment> payments = new List<Payment>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            SELECT p.payment_id, p.paid_amount, p.paid_date, p.payment_method, p.status, p.fk_job_id
            FROM payment p
            JOIN job j ON p.fk_job_id = j.job_id
            WHERE j.fk_user_id = @userId";  // adjust to your schema

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payments.Add(new Payment
                            {
                                PaymentId = Convert.ToInt32(reader["payment_id"]),
                                PaidAmount = Convert.ToDecimal(reader["paid_amount"]),
                                PaidDate = Convert.ToDateTime(reader["paid_date"]),
                                PaymentMethod = reader["payment_method"].ToString(),
                                Status = reader["status"].ToString(),
                                JobId = Convert.ToInt32(reader["fk_job_id"])
                            });
                        }
                    }
                }
            }

            return payments;
        }
        public int GetPendingPayments()
        {
            int count = 0;
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM payment WHERE status = 'pending'";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return count;
        }
        public Dictionary<string, float> GetMonthlyRevenue()
        {
            var revenueByMonth = new Dictionary<string, float>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            SELECT 
                DATE_FORMAT(paid_date, '%Y-%m') AS month,
                SUM(paid_amount) AS total
            FROM payment
            WHERE status = 'completed'
            GROUP BY month
            ORDER BY month;
        ";

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string month = reader.GetString("month");
                        float total = reader.GetFloat("total");
                        revenueByMonth.Add(month, total);
                    }
                }
            }

            return revenueByMonth;
        }
        public Payment GetPaymentByJobId(int jobId)
        {
            Payment payment = null;
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM payment WHERE fk_job_id = @jobId";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@jobId", jobId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            payment = new Payment
                            {
                                PaymentId = reader.GetInt32("payment_id"),
                                PaidAmount = reader.GetDecimal("paid_amount"),
                                PaidDate = reader.GetDateTime("paid_date"),
                                PaymentMethod = reader.GetString("payment_method"),
                                Status = reader.GetString("status"),
                                JobId = reader.GetInt32("fk_job_id")
                            };
                        }
                    }
                }
            }
            return payment;
        }



    }
}
