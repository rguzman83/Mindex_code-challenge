using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using challenge.Data;
using System.Threading;

namespace challenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly CompensationContext _compositionContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext, CompensationContext compensationContext)
        {
            _employeeContext = employeeContext;
            _compositionContext = compensationContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        public Employee GetById(string id)
        {
            /*
            var tempHolder = _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == id);
            _employeeContext.Entry(tempHolder).Collection(e => e.DirectReports).Load();
            return tempHolder;
            */
            //return _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == id);

            //IEnumerable<Employee> tempHoldings = _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == id);
            //return _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == id);

            //Due to Lazy loading issues with Entity Frameowrk, .Include is necessary to reference DirectReports
            var employee = _employeeContext.Employees.Include(e => e.DirectReports).SingleOrDefault(e => e.EmployeeId == id);
            if (employee == null)
                return null;
            LoadDirectReports(employee);
            return employee;
        }

        public Compensation GetCompById(string id)
        {
            var employee = _compositionContext.Compensations.SingleOrDefault(c => c.EmployeeID == id);
            if (employee == null)
                return null;
            return employee;
        }


        public Compensation AddComp(Compensation compensation)
        {
            _compositionContext.Compensations.Add(compensation);
            return compensation;
        }

        private void LoadDirectReports(Employee employee)
        {
            if (employee != null)
            {
                _employeeContext.Entry(employee).Collection(e => e.DirectReports).Load();

                foreach (var directReport in employee.DirectReports)
                {
                    LoadDirectReports(directReport);
                }
            }
        }

        public ReportingStructure GetReportStructure(string id)
        {
            //Direct Copy of Get Worker Code for the moment
            var employee = new ReportingStructure();
            var starterEmployee = _employeeContext.Employees.Include(e => e.DirectReports).SingleOrDefault(e => e.EmployeeId == id);
            LoadDirectReports(starterEmployee);
            employee.Employee = starterEmployee;
            //employee.NumberOfReports = 1;
            employee.NumberOfReports = GetReportCount(starterEmployee, 0);
            //LoadReportStruct(employee);
            return employee;

        }

        public int GetReportCount(Employee employee, int intCount)
        {
            if (employee != null)
            {
                Console.WriteLine("EE = " + employee.FirstName);
                if (employee.DirectReports != null)
                    Console.WriteLine("DR Count = " + employee.DirectReports.Count);
                //int intCount = 0;
                //employee.NumberOfReports = employee.Employee.DirectReports.Count;
                //employee.Employee.FirstName = "UPDATED";
                if (employee.DirectReports != null)
                {
                    foreach (var directReport in employee.DirectReports)
                    {
                        Console.WriteLine(directReport.EmployeeId);
                        intCount += 1 + GetReportCount(directReport, 0);
                    }

                }
            }
            return intCount;
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Task CompSaveAsync()
        {
            return _compositionContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
    }
}
