using EShift_Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Business.Interface
{
    public interface ILoadService
    {
        bool SaveLoadWithContainers(Load load, List<int> containerIds);
        List<Load> GetLoadsByJobId(int jobId);
        List<Container> GetAvailableContainersBySize(string size);
        bool UpdateLoadWithContainers(Load load, List<int> containerIds);
        bool DeleteLoad(int loadId);
        List<int> GetContainerIdsByLoadId(int loadId);

    }
}
