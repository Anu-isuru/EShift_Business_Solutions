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
    public class ContainerRepository : IContainerRepository
    {
        private string connectionString = "Server=localhost;Database=eshift_db;Uid=root;Pwd=1234;";
        public List<Container> GetAllAvailableContainers()
        {
            List<Container> containers = new List<Container>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM container WHERE status = 'available'";
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        containers.Add(new Container
                        {
                            ContainerId = reader.GetInt32("container_id"),
                            Size = reader.GetString("size"),
                            Price = reader.GetFloat("price"),
                            Status = reader.GetString("status"),
                            Condition = reader.GetString("condition")
                        });
                    }
                }
            }
            return containers;
        }
    
    public void UpdateStatus(int containerId, string status)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE container SET status = @status WHERE container_id = @id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@id", containerId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Container> GetAvailableContainersBySize(string size)
        {
            List<Container> containers = new List<Container>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM container WHERE size = @size AND status = 'available'";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@size", size);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            containers.Add(new Container
                            {
                                ContainerId = Convert.ToInt32(reader["container_id"]),
                                Size = reader["size"].ToString(),
                                Price = (float)Convert.ToDouble(reader["price"]),
                                Status = reader["status"].ToString(),
                                Condition = reader["condition"].ToString()
                            });
                        }
                    }
                }
            }

            return containers;
        }
        public void MarkContainerAsAvailable(int containerId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE container SET status = 'available' WHERE container_id = @id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", containerId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
