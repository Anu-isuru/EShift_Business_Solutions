using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EShift_Business.Models;

namespace EShift_Business.Repository.Interface
{
    public interface IStaffRepository
    {
        bool AddStaff(Staff staff);
        List<Staff> GetAllStaff();
        List<Staff> GetAvailableDrivers();
        List<Staff> GetAvailableAssistants();

    }
}
