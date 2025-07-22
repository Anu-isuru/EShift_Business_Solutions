using EShift_Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShift_Business.Business.Interface
{
    public interface IStaffService
    {
        bool AddStaff(Staff staff);
        List<Staff> GetAllStaff();
        bool UpdateStaff(Staff staff);
        bool DeleteStaff(int staffId);
        Staff GetStaffById(int staffId);
        List<Staff> GetAvailableDrivers();
        List<Staff> GetAvailableAssistants();
        bool UpdateStaffAvailability(int staffId, string newStatus);
    }
}
