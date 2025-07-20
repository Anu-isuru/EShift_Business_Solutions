using EShift_Business.Models;
using EShift_Business.Repository.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Repository.Interface
{
    public interface ILoadRepository
    {
        int SaveLoad(Load load);
        List<Load> GetLoadsByJobId(int jobId);
        void AssignContainersToLoad(int loadId, List<int> containerIds);

        bool UpdateLoadWithContainers(Load load, List<int> containerIds);
        bool DeleteLoad(int loadId);

        List<int> GetContainerIdsByLoadId(int loadId);
    }
}
