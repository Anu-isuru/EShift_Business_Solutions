using EShift_Business.Business.Interface;
using EShift_Business.Models;
using EShift_Business.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Business.Services
{
    public class LorryService : ILorryService
    {
        private readonly ILorryRepository _lorryRepository;
        public LorryService(ILorryRepository lorryRepository)
        {
            _lorryRepository = lorryRepository;
        }
        public int GetAvailableLorries()
        {
            return _lorryRepository.GetAvailableLorries();
        }
        public List<Lorry> GetAvailableLorriestoAssign()
        {
            return _lorryRepository.GetAvailableLorriestoAssign();
        }
        public bool UpdateLorryAvailability(int lorryId, string status)
        {
            return _lorryRepository.UpdateAvailability(lorryId, status);
        }

    }
}
