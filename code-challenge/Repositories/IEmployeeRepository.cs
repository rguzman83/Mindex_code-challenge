using challenge.Models;
using System;
using System.Threading.Tasks;

namespace challenge.Repositories
{
    public interface IEmployeeRepository
    {
        Employee GetById(String id);
        ReportingStructure GetReportStructure(String id);
        Employee Add(Employee employee);
        Employee Remove(Employee employee);
        Task SaveAsync();
    }
}