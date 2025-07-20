using EShift_Business.Models;
using EShift_Business.Repository.Interface;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EShift_Business.Repository.Services
{
    public class LoadRepository : ILoadRepository
    {
        private string connectionString = "Server=localhost;Database=eshift_db;Uid=root;Pwd=1234;";

        public void AssignContainersToLoad(int loadId, List<int> containerIds)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                foreach (var containerId in containerIds)
                {
                    // Insert into mapping table
                    string mapQuery = "INSERT INTO load_container (load_id, container_id) VALUES (@loadId, @containerId);";
                    using (var cmd = new MySqlCommand(mapQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@loadId", loadId);
                        cmd.Parameters.AddWithValue("@containerId", containerId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public List<Load> GetLoadsByJobId(int jobId)
        {
            var loads = new List<Load>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT l.load_id, l.description, l.size, l.status, lc.container_id
                         FROM `load` l
                         LEFT JOIN load_container lc ON l.load_id = lc.load_id
                         WHERE l.fk_job_id = @jobId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@jobId", jobId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int loadId = Convert.ToInt32(reader["load_id"]);
                            var existingLoad = loads.FirstOrDefault(x => x.LoadId == loadId);

                            if (existingLoad == null)
                            {
                                existingLoad = new Load
                                {
                                    LoadId = loadId,
                                    Description = reader["description"].ToString(),
                                    Size = reader["size"].ToString(),
                                    Status = reader["status"].ToString(),
                                };
                                loads.Add(existingLoad);
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("container_id")))
                                existingLoad.ContainerIds.Add(Convert.ToInt32(reader["container_id"]));
                        }
                    }
                }
            }
            return loads;
        }

        public int SaveLoad(Load load)
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO `load` (description, size, status, fk_job_id) 
                         VALUES (@desc, @size, @status, @jobId); 
                         SELECT LAST_INSERT_ID();"; // Using backticks around `load`
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@desc", load.Description ?? "");
                        cmd.Parameters.AddWithValue("@size", load.Size ?? "");
                        cmd.Parameters.AddWithValue("@status", load.Status ?? "assigned");
                        cmd.Parameters.AddWithValue("@jobId", load.JobId);

                        int loadId = Convert.ToInt32(cmd.ExecuteScalar());
                        Console.WriteLine("Inserted Load ID: " + loadId); // Debug log
                        return loadId;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in SaveLoad: " + ex.Message);
                return -1; // Return -1 to indicate failure
            }
        }

        public void UpdateContainerStatus(int containerId, string status)
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
        public bool UpdateLoadWithContainers(Load load, List<int> containerIds)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Update load info
                        string updateQuery = "UPDATE load SET description = @desc, size = @size WHERE load_id = @loadId";
                        using (var cmd = new MySqlCommand(updateQuery, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@desc", load.Description);
                            cmd.Parameters.AddWithValue("@size", load.Size);
                            cmd.Parameters.AddWithValue("@loadId", load.LoadId);
                            cmd.ExecuteNonQuery();
                        }

                        // Clear old container assignments
                        string deleteQuery = "DELETE FROM load_container WHERE load_id = @loadId";
                        using (var cmd = new MySqlCommand(deleteQuery, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@loadId", load.LoadId);
                            cmd.ExecuteNonQuery();
                        }

                        // Assign new containers
                        foreach (int cid in containerIds)
                        {
                            string insert = "INSERT INTO load_container (load_id, container_id) VALUES (@lid, @cid)";
                            using (var cmd = new MySqlCommand(insert, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@lid", load.LoadId);
                                cmd.Parameters.AddWithValue("@cid", cid);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();
                        return true;
                    }
                    catch
                    {
                        tran.Rollback();
                        return false;
                    }
                }
            }
        }

        public bool DeleteLoad(int loadId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Get container IDs assigned to this load
                        List<int> containerIds = new List<int>();
                        string getContainersQuery = "SELECT container_id FROM load_container WHERE load_id = @id";
                        using (var cmdGet = new MySqlCommand(getContainersQuery, conn, transaction))
                        {
                            cmdGet.Parameters.AddWithValue("@id", loadId);
                            using (var reader = cmdGet.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    containerIds.Add(reader.GetInt32("container_id"));
                                }
                            }
                        }

                        // 2. Set container statuses to 'available'
                        foreach (int containerId in containerIds)
                        {
                            string updateStatusQuery = "UPDATE container SET status = 'available' WHERE container_id = @containerId";
                            using (var cmdUpdate = new MySqlCommand(updateStatusQuery, conn, transaction))
                            {
                                cmdUpdate.Parameters.AddWithValue("@containerId", containerId);
                                cmdUpdate.ExecuteNonQuery();
                            }
                        }

                        // 3. Delete mappings from load_container
                        string deleteMappingQuery = "DELETE FROM load_container WHERE load_id = @id";
                        using (var cmd1 = new MySqlCommand(deleteMappingQuery, conn, transaction))
                        {
                            cmd1.Parameters.AddWithValue("@id", loadId);
                            cmd1.ExecuteNonQuery();
                        }

                        // 4. Delete load
                        string deleteLoadQuery = "DELETE FROM `load` WHERE load_id = @id";
                        using (var cmd2 = new MySqlCommand(deleteLoadQuery, conn, transaction))
                        {
                            cmd2.Parameters.AddWithValue("@id", loadId);
                            cmd2.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error deleting load: " + ex.Message);
                        return false;
                    }
                }
            }
        }

            public List<int> GetContainerIdsByLoadId(int loadId)
        {
            var ids = new List<int>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT container_id FROM load_container WHERE load_id = @id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", loadId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ids.Add(Convert.ToInt32(reader["container_id"]));
                        }
                    }
                }
            }
            return ids;
        }

    }
}
