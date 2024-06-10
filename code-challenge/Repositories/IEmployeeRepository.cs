using challenge.Models;
using System;
using System.Threading.Tasks;

namespace challenge.Repositories
{
    public interface IEmployeeRepository
    {
        Employee GetById(String id);
        Employee Add(Employee employee);
        Employee Remove(Employee employee);
        ReportingStructure GetReportStructure(String id);
        Compensation GetCompById(String id);
        Compensation AddComp(Compensation compensation);
        Task SaveAsync();
        Task CompSaveAsync();
    }
}