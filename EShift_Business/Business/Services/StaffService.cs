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
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;

        public StaffService(IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }

        public bool AddStaff(Staff staff)
        {
            return _staffRepository.AddStaff(staff);
        }

        public List<Staff> GetAllStaff()
        {
            return _staffRepository.GetAllStaff();
        }


        public bool DeleteStaff(int staffId)
        {
            throw new NotImplementedException();
           // return _staffRepository.DeleteStaff(staffId);
        }

        public Staff GetStaffById(int staffId)
        {
            throw new NotImplementedException();
            //return _staffRepository.GetStaffById(staffId);
        }

        public bool UpdateStaff(Staff staff)
        {
            throw new NotImplementedException();
        }
        public List<Staff> GetAvailableDrivers()
        {
            return _staffRepository.GetAvailableDrivers();
        }

        public List<Staff> GetAvailableAssistants()
        {
            return _staffRepository.GetAvailableAssistants();
        }
        public bool UpdateStaffAvailability(int staffId, string newStatus)
        {
            return _staffRepository.UpdateAvailability(staffId, newStatus);
        }
    }
}
