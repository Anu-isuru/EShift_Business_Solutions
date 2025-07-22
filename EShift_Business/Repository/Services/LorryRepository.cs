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
    public class LorryRepository : ILorryRepository
    {
        private string connectionString = "Server=localhost;Database=eshift_db;Uid=root;Pwd=1234";
        public int GetAvailableLorries()
        {
            int count = 0;
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM lorry WHERE availability_state = 'available'";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return count;
        }
        public List<Lorry> GetAvailableLorriestoAssign()
        {
            var list = new List<Lorry>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM lorry WHERE availability_state = 'available'";
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Lorry
                        {
                            Id = reader.GetInt32("lorry_id"),
                            Registration_No = reader.GetString("registration_no"),
                            Capacity = reader.GetString("capacity"),
                            Condition = reader.GetString("condition"),
                            Availability_State = reader.GetString("availability_state")
                        });
                    }
                }
            }
            return list;
        }
        public bool UpdateAvailability(int lorryId, string status)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE lorry SET availability_state = @status WHERE lorry_id = @id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@id", lorryId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

    }
}
