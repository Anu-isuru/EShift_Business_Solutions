using EShift_Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Business.Interface
{
    public interface IContainerService
    {
        List<Container> GetAvailableContainers();
        void MarkContainerAsAvailable(int containerId);

    }
}
