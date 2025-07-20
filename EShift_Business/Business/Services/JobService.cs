using EShift_Business.Business.Interface;
using EShift_Business.Models;
using EShift_Business.Repository.Interface;
using EShift_Business.Repository.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Business.Services
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;

        public JobService(IJobRepository repo)
        {
            _jobRepository = repo;
        }

        public int AddLocation(string address, string city, string province)
        {
            return _jobRepository.AddLocation(address, city, province);
        }

        public int SaveJob(Job job)
        {
            return _jobRepository.SaveJob(job);
        }

        public List<Job> GetJobsByUserId(int userId)
        {
            return _jobRepository.GetJobsByUserId(userId);
        }
        public List<Job> GetJobHistoryWithLocations(int userId)
        {
            return _jobRepository.GetJobHistoryWithLocations(userId);
        }
        public bool UpdateJob(Job job)
        {
            return _jobRepository.UpdateJob(job);
        }
        public Job GetJobById(int jobId)
        {
            return _jobRepository.GetJobById(jobId);
        }
        public bool DeleteJob(int jobId)
        {
            return _jobRepository.DeleteJob(jobId);
        }
        public int GetTotalJobs()
        {
            return _jobRepository.GetTotalJobs();
        }
        public int GetCompletedJobs()
        {
            return _jobRepository.GetCompletedJobs();
        }
        public Dictionary<string, int> GetJobCountByStatus()
        {
            return _jobRepository.GetJobCountByStatus();
        }
        public List<Job> GetPendingJobs()
        {
            return _jobRepository.GetPendingJobs();
        }


    }
}
