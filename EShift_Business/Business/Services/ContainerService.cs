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
    public class ContainerService : IContainerService
    {
        private readonly IContainerRepository _containerRepo;
        public ContainerService()
        {
            _containerRepo = new ContainerRepository(); // You can inject via constructor
        }

        public List<Container> GetAvailableContainers()
        {
            return _containerRepo.GetAllAvailableContainers();
        }
        public void MarkContainerAsAvailable(int id) => _containerRepo.MarkContainerAsAvailable(id);

    }
}
