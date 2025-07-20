using EShift_Business.Models;
using EShift_Business.Repository.Interface;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Repository.Services
{
    public class CustomerRepository : ICustomerRepository
    {
        string connectionString = "Server=localhost;Database=eshift_db;Uid=root;Pwd=1234;";
        public Customer GetCustomerByEmail(string email)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM customer_details WHERE email = @Email";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Customer
                            {
                                CustomerId = Convert.ToInt32(reader["customer_id"]),
                                FirstName = reader["first_name"].ToString(),
                                LastName = reader["last_name"].ToString(),
                                Email = reader["email"].ToString(),
                                ContactNo = reader["contact_no"].ToString(),
                               // Password = reader["password"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }
        public void RegisterCustomer(Customer customer)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO customer_details (f_name, l_name, email, contact_no, registration_date, fk_user_id) " +
                       "VALUES (@FirstName, @LastName, @Email, @ContactNo, @RegistrationDate, @UserId)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", customer.LastName);
                    cmd.Parameters.AddWithValue("@Email", customer.Email);
                    cmd.Parameters.AddWithValue("@ContactNo", customer.ContactNo);
                    cmd.Parameters.AddWithValue("@RegistrationDate", customer.RegistrationDate);
                    cmd.Parameters.AddWithValue("@UserId", customer.UserId); // Make sure this is set before calling

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public Customer GetCustomerByUserId(int userId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT customer_id, f_name, l_name, contact_no, fk_user_id FROM customer_details WHERE fk_user_id = @UserId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Customer
                            {
                                CustomerId = Convert.ToInt32(reader["customer_id"]),
                                FirstName = reader["f_name"].ToString(),
                                LastName = reader["l_name"].ToString(),
                                ContactNo = reader["contact_no"].ToString(),
                                UserId = Convert.ToInt32(reader["fk_user_id"])
                            };
                        }
                    }
                }
            }
            return null;
        }
        public void UpdateCustomer(Customer customer)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE customer_details SET f_name = @FirstName, l_name = @LastName, contact_no = @ContactNo WHERE customer_id = @CustomerId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", customer.LastName);
                    cmd.Parameters.AddWithValue("@ContactNo", customer.ContactNo);
                    cmd.Parameters.AddWithValue("@CustomerId", customer.CustomerId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
