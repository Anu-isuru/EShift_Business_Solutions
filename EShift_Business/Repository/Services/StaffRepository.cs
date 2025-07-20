using EShift_Business.Models;
using EShift_Business.Repository.Interface;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Repository.Services
{
    public class StaffRepository : IStaffRepository
    {
        private string connectionString = "Server=localhost;Database=eshift_db;Uid=root;Pwd=1234;";
        public bool AddStaff(Staff staff)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"INSERT INTO staff_details 
                         (f_name, l_name, contact_no, email, license_number, availability_status, fk_user_id, created_date)
                         VALUES 
                         (@fName, @lName, @contact, @Email, @license, @status, @userId, @createdDate)";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@fName", staff.FirstName);
                    cmd.Parameters.AddWithValue("@lName", staff.LastName);
                    cmd.Parameters.AddWithValue("@contact", staff.ContactNo);
                    cmd.Parameters.AddWithValue("@Email", staff.Email);
                    cmd.Parameters.AddWithValue("@license", staff.LicenseNumber);
                    cmd.Parameters.AddWithValue("@status", staff.AvailabilityStatus);
                    cmd.Parameters.AddWithValue("@userId", staff.UserId);
                    cmd.Parameters.AddWithValue("@createdDate", staff.CreatedDate);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }

        }
        public List<Staff> GetAllStaff()
        {
            List<Staff> staffList = new List<Staff>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT s.staff_id, s.f_name, s.l_name, s.contact_no, s.email, 
                                s.license_number, s.availability_status, s.created_date,
                                u.user_id, u.role, u.is_active
                         FROM staff_details s
                         JOIN user u ON s.fk_user_id = u.user_id";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var staff = new Staff
                            {
                                StaffId = Convert.ToInt32(reader["staff_id"]),
                                FirstName = reader["f_name"].ToString(),
                                LastName = reader["l_name"].ToString(),
                                ContactNo = reader["contact_no"].ToString(),
                                Email = reader["email"].ToString(),
                                LicenseNumber = reader["license_number"].ToString(),
                                AvailabilityStatus = reader["availability_status"].ToString(),
                                CreatedDate = Convert.ToDateTime(reader["created_date"]),
                                UserId = Convert.ToInt32(reader["user_id"]),
                                Role = reader["role"].ToString(),
                            };

                            staffList.Add(staff);

                        }
                    }
                }
            }
            return staffList;
        }

        public List<Staff> GetAvailableDrivers()
        {
            List<Staff> drivers = new List<Staff>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
    SELECT 
        s.staff_id,
        s.f_name,
        s.l_name,
        s.contact_no,
        s.email,
        s.availability_status,
        u.role
    FROM staff_details s
    JOIN user u ON s.fk_user_id = u.user_id
    WHERE s.availability_status = 'available' AND u.role = 'driver';"; 

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        drivers.Add(new Staff
                        {
                            StaffId = reader.GetInt32("staff_id"),
                            FirstName = reader.GetString("f_name"),      
                            LastName = reader.GetString("l_name"),       
                            ContactNo = reader.GetString("contact_no"),
                            Email = reader.GetString("email"),
                            AvailabilityStatus = reader.GetString("availability_status"),
                            Role = reader.GetString("role")
                        });
                    }
                }
            }

            return drivers;
        }
        public List<Staff> GetAvailableAssistants()
        {
            var assistant = new List<Staff>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"    SELECT 
        s.staff_id,
        s.f_name,
        s.l_name,
        s.contact_no,
        s.email,
        s.availability_status,
        u.role
    FROM staff_details s
    JOIN user u ON s.fk_user_id = u.user_id
    WHERE s.availability_status = 'available' AND u.role = 'assistant'; "; 

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        assistant.Add(new Staff
                        {
                            StaffId = reader.GetInt32("staff_id"),
                            FirstName = reader.GetString("f_name"),      
                            LastName = reader.GetString("l_name"),       
                            ContactNo = reader.GetString("contact_no"),
                            Email = reader.GetString("email"),
                            AvailabilityStatus = reader.GetString("availability_status"),
                            Role = reader.GetString("role")
                        });
                    }
                }
            }
            return assistant;
        }
    }
}