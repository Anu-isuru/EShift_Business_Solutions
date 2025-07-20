using EShift_Business.Business.Interface;
using EShift_Business.Models;
using EShift_Business.Repository.Interface;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EShift_Business.Business.Services
{
    public class LoadService : ILoadService

    {
        private readonly ILoadRepository _loadRepository;
        private readonly IContainerRepository _containerRepository;

        public LoadService(ILoadRepository loadRepository, IContainerRepository containerRepository)
        {
            _loadRepository = loadRepository;
            _containerRepository = containerRepository;
        }

        public bool SaveLoadWithContainers(Load load, List<int> containerIds)
        {
            try
            {
                int loadId = _loadRepository.SaveLoad(load);
                if (loadId == -1) // Check if SaveLoad failed
                {
                    Console.WriteLine("Failed to save load.");
                    return false;
                }

                foreach (int containerId in containerIds)
                {
                    _containerRepository.UpdateStatus(containerId, "assigned");
                    MessageBox.Show("Updating container: " + containerId);
                }
                _loadRepository.AssignContainersToLoad(loadId, containerIds);
                MessageBox.Show("Load-container mapping done.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in SaveLoadWithContainers: " + ex.Message);
                return false;
            }
        }

        public List<Load> GetLoadsByJobId(int jobId)
        {
            return _loadRepository.GetLoadsByJobId(jobId);
        }

        public List<Container> GetAvailableContainersBySize(string size)
        {
            return _containerRepository.GetAvailableContainersBySize(size);
        }
        public bool UpdateLoadWithContainers(Load load, List<int> containerIds)
        {
            return _loadRepository.UpdateLoadWithContainers(load, containerIds);
        }
        public bool DeleteLoad(int loadId) => _loadRepository.DeleteLoad(loadId);
        public List<int> GetContainerIdsByLoadId(int loadId) => _loadRepository.GetContainerIdsByLoadId(loadId);

    }
}
