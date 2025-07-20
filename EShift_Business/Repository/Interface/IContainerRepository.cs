using EShift_Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Repository.Interface
{
    public interface IContainerRepository
    {
        List<Container> GetAllAvailableContainers();
        void UpdateStatus(int containerId, string status);
        List<Container> GetAvailableContainersBySize(string size);
        void MarkContainerAsAvailable(int containerId);
    }
}
