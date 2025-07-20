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
    public class JobRepository : IJobRepository
    {
        private string connectionString = "Server=localhost;Database=eshift_db;Uid=root;Pwd=1234;";

        public int AddLocation(string address, string city, string province)
        {
            int locationId = 0;
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO location (address, city, province) VALUES (@address, @city, @province); SELECT LAST_INSERT_ID();";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@address", address);
                    cmd.Parameters.AddWithValue("@city", city);
                    cmd.Parameters.AddWithValue("@province", province);
                    locationId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return locationId;
        }

        public int SaveJob(Job job)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO job 
            (fk_pickup_location_id, fk_dropoff_location_id, fk_user_id, pickup_date, dropoff_date, status) 
            VALUES 
            (@pickup, @dropoff, @user, @pickupDate, @dropoffDate, @status); 
            SELECT LAST_INSERT_ID();";  // ✅ This returns the inserted job_id

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pickup", job.PickupLocationId);
                    cmd.Parameters.AddWithValue("@dropoff", job.DropoffLocationId);
                    cmd.Parameters.AddWithValue("@user", job.UserId);
                    cmd.Parameters.AddWithValue("@pickupDate", job.PickupDate);
                    cmd.Parameters.AddWithValue("@dropoffDate", job.DropoffDate);
                    cmd.Parameters.AddWithValue("@status", job.Status ?? "pending");

                    int insertedJobId = Convert.ToInt32(cmd.ExecuteScalar());  // ✅ Now it works!
                    return insertedJobId;
                }
            }
        }

        public List<Job> GetJobsByUserId(int userId)
        {
            List<Job> jobs = new List<Job>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM job WHERE fk_user_id = @userId ORDER BY pickup_date DESC";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            jobs.Add(new Job
                            {
                                JobId = Convert.ToInt32(reader["job_id"]),
                                PickupDate = Convert.ToDateTime(reader["pickup_date"]),
                                DropoffDate = Convert.ToDateTime(reader["dropoff_date"]),
                                Status = reader["status"].ToString()
                            });
                        }
                    }
                }
            }
            return jobs;
        }

        public List<Job> GetJobHistoryWithLocations(int userId)
        {
            var jobList = new List<Job>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
        SELECT 
            j.job_id,
            j.fk_pickup_location_id,
            j.fk_dropoff_location_id,
            j.pickup_date,
            j.dropoff_date,
            j.status,
            p.address AS pickup_address,
            p.city AS pickup_city,
            p.province AS pickup_province,
            d.address AS dropoff_address,
            d.city AS dropoff_city,
            d.province AS dropoff_province
        FROM job j
        JOIN location p ON j.fk_pickup_location_id = p.location_id
        JOIN location d ON j.fk_dropoff_location_id = d.location_id
        WHERE j.fk_user_id = @UserId
        ORDER BY j.pickup_date DESC";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            jobList.Add(new Job
                            {
                                JobId = Convert.ToInt32(reader["job_id"]),
                                PickupLocationId = Convert.ToInt32(reader["fk_pickup_location_id"]),  // ✅ Added
                                DropoffLocationId = Convert.ToInt32(reader["fk_dropoff_location_id"]), // ✅ Added
                                PickupDate = Convert.ToDateTime(reader["pickup_date"]),
                                DropoffDate = Convert.ToDateTime(reader["dropoff_date"]),
                                Status = reader["status"].ToString(),
                                PickupAddress = reader["pickup_address"].ToString(),
                                PickupCity = reader["pickup_city"].ToString(),
                                PickupProvince = reader["pickup_province"].ToString(),
                                DropoffAddress = reader["dropoff_address"].ToString(),
                                DropoffCity = reader["dropoff_city"].ToString(),
                                DropoffProvince = reader["dropoff_province"].ToString()
                            });
                        }
                    }
                }
            }
            return jobList;
        }


        public Job GetJobById(int jobId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            SELECT 
                j.*, 
                p.address AS pickup_address, p.city AS pickup_city, p.province AS pickup_province,
                d.address AS dropoff_address, d.city AS dropoff_city, d.province AS dropoff_province
            FROM job j
            JOIN location p ON j.fk_pickup_location_id = p.location_id
            JOIN location d ON j.fk_dropoff_location_id = d.location_id
            WHERE j.job_id = @jobId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@jobId", jobId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Job
                            {
                                JobId = Convert.ToInt32(reader["job_id"]),
                                UserId = Convert.ToInt32(reader["fk_user_id"]),
                                PickupLocationId = Convert.ToInt32(reader["fk_pickup_location_id"]),
                                DropoffLocationId = Convert.ToInt32(reader["fk_dropoff_location_id"]),
                                PickupDate = Convert.ToDateTime(reader["pickup_date"]),
                                DropoffDate = Convert.ToDateTime(reader["dropoff_date"]),
                                Status = reader["status"].ToString(),

                                // Optional: Fill pickup/dropoff details
                                PickupAddress = reader["pickup_address"].ToString(),
                                PickupCity = reader["pickup_city"].ToString(),
                                PickupProvince = reader["pickup_province"].ToString(),
                                DropoffAddress = reader["dropoff_address"].ToString(),
                                DropoffCity = reader["dropoff_city"].ToString(),
                                DropoffProvince = reader["dropoff_province"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }


        public bool UpdateJob(Job job)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            UPDATE job 
            SET fk_pickup_location_id = @pickup, 
                fk_dropoff_location_id = @dropoff,
                pickup_date = @pickupDate,
                dropoff_date = @dropoffDate,
                status = @status
            WHERE job_id = @jobId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pickup", job.PickupLocationId);
                    cmd.Parameters.AddWithValue("@dropoff", job.DropoffLocationId);
                    cmd.Parameters.AddWithValue("@pickupDate", job.PickupDate);
                    cmd.Parameters.AddWithValue("@dropoffDate", job.DropoffDate);
                    cmd.Parameters.AddWithValue("@status", job.Status ?? "pending");
                    cmd.Parameters.AddWithValue("@jobId", job.JobId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteJob(int jobId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // 1. Check if any loads exist under this job
                string checkLoadsQuery = "SELECT COUNT(*) FROM `load` WHERE fk_job_id = @jobId";
                using (var cmdCheck = new MySqlCommand(checkLoadsQuery, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@jobId", jobId);
                    int loadCount = Convert.ToInt32(cmdCheck.ExecuteScalar());

                    if (loadCount > 0)
                    {
                        // Loads still exist — stop deletion and inform the user
                        MessageBox.Show("Please delete all associated loads before deleting the job.", "Cannot Delete Job", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

                // 2. Delete the job
                string deleteJobQuery = "DELETE FROM job WHERE job_id = @jobId";
                using (var cmdDelete = new MySqlCommand(deleteJobQuery, conn))
                {
                    cmdDelete.Parameters.AddWithValue("@jobId", jobId);
                    return cmdDelete.ExecuteNonQuery() > 0;
                }
            }
        }
        public int GetTotalJobs()
        {
            int count = 0;
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM job";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return count;
        }
        public int GetCompletedJobs()
        {
            int count = 0;
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM job WHERE status = 'completed'";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return count;
        }
        public Dictionary<string, int> GetJobCountByStatus()
        {
            var jobCounts = new Dictionary<string, int>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            SELECT status, COUNT(*) AS count 
            FROM job 
            GROUP BY status;
        ";

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string status = reader.GetString("status");
                        int count = reader.GetInt32("count");
                        jobCounts[status] = count;
                    }
                }
            }

            return jobCounts;
        }

        public List<Job> GetPendingJobs()
        {
            List<Job> jobs = new List<Job>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
    SELECT 
        j.job_id,
        j.pickup_date,
        j.dropoff_date,
        j.status,
        j.fk_user_id,
        j.fk_pickup_location_id,
        j.fk_dropoff_location_id,

        -- Pickup location
        CONCAT(p.address, ', ', p.city, ', ', p.province) AS PickupAddress,

        -- Dropoff location
        CONCAT(d.address, ', ', d.city, ', ', d.province) AS DropoffAddress

    FROM job j
    JOIN location p ON j.fk_pickup_location_id = p.location_id
    JOIN location d ON j.fk_dropoff_location_id = d.location_id
    WHERE j.status = 'Pending';";

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Job job = new Job
                        {
                            JobId = reader.GetInt32("job_id"),
                            PickupDate = reader.GetDateTime("pickup_date"),
                            DropoffDate = reader.GetDateTime("dropoff_date"),
                            Status = reader.GetString("status"),
                            UserId = reader.GetInt32("fk_user_id"),

                            PickupAddress = reader.GetString("PickupAddress"),
                            DropoffAddress = reader.GetString("DropoffAddress")
                        };
                        jobs.Add(job);
                    }
                }
            }
            return jobs;
        }



    }
}
