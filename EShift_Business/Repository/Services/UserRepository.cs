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
    public class UserRepository : IUserRepository
    {
        string connectionString = "Server=localhost;Database=eshift_db;Uid=root;Pwd=1234;";
        public int RegisterUser(User user)
        {
            int newUserId = 0;

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"INSERT INTO user (email, password, role, is_active)
                         VALUES (@Email, @Password, @Role, @IsActive);
                         SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@Role", user.Role);
                    cmd.Parameters.AddWithValue("@IsActive", user.IsActive);

                    newUserId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return newUserId;
        }

        public User GetUser(string email, string password, string role)
        {
            User user = null;

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"SELECT * FROM user 
                         WHERE email = @Email AND password = @Password AND role = @Role AND is_active = 1";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password); // Hash later
                    cmd.Parameters.AddWithValue("@Role", role);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                UserId = Convert.ToInt32(reader["user_id"]),
                                Email = reader["email"].ToString(),
                                Password = reader["password"].ToString(),
                                Role = reader["role"].ToString(),
                                IsActive = Convert.ToBoolean(reader["is_active"])
                            };
                        }
                    }
                }
            }

            return user;
        }
        public int GetActiveCustomers()
        {
            int count = 0;
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM user WHERE is_active = '1'";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return count;
        }

    }
}