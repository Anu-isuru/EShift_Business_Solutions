using EShift_Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Repository.Interface
{
    public interface IJobRepository
    {
        int AddLocation(string address, string city, string province);
        int SaveJob(Job job);
        List<Job> GetJobsByUserId(int userId);
        List<Job> GetJobHistoryWithLocations(int userId);
        Job GetJobById(int jobId);
        bool UpdateJob(Job job);
        bool DeleteJob(int jobId);
        int GetTotalJobs();
        int GetCompletedJobs();
        Dictionary<string, int> GetJobCountByStatus();
        List<Job> GetPendingJobs();
        bool UpdateJobStatus(int jobId, string newStatus);
        List<JobReportDTO> GetAllJobDetailsWithCustomer();
        List<JobReportDTO> GetJobsByStatus(string status);

    }
}
