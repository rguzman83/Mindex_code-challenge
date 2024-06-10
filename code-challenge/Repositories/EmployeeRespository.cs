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
        private readonly CompensationContext _compensationContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext, CompensationContext compensationContext)
        {
            _employeeContext = employeeContext;
            _compensationContext = compensationContext;
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

            //Due to Lazy loading issues with Entity Frameowrk, .Include is necessary to reference DirectReports
            var employee = _employeeContext.Employees.Include(e => e.DirectReports).SingleOrDefault(e => e.EmployeeId == id);
            if (employee == null)
                return null;
            LoadDirectReports(employee);
            return employee;
        }

        public Compensation GetCompById(string id)
        {
            Compensation employee = _compensationContext.Compensations.Include(c => c.Employee).SingleOrDefault(c => c.Employee.EmployeeId == id);
            if (employee == null)
                return null;
            return employee;
        }


        public Compensation AddComp(CompensationPost compensation, string id)
        {
            var employee = _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == id);
            Compensation employeeCompensation = _compensationContext.Compensations.SingleOrDefault(c => c.Employee.EmployeeId == id);
            if (employee == null)
                return null;
            if (employeeCompensation == null)
            {
                Compensation newRecord = new Compensation
                {
                    Employee = employee,
                    Salary = compensation.Salary,
                    EffectiveDate = compensation.EffectiveDate
                };
                employeeCompensation = newRecord;
                _compensationContext.Compensations.Add(employeeCompensation);
            }
            else //Normally this would be in a sperate PUT/PATCH action - putting here just as PoC
            {
                employeeCompensation.Salary = compensation.Salary;
                employeeCompensation.EffectiveDate = compensation.EffectiveDate;
            }
            return employeeCompensation;
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
            var employee = new ReportingStructure();
            var starterEmployee = _employeeContext.Employees.Include(e => e.DirectReports).SingleOrDefault(e => e.EmployeeId == id);
            LoadDirectReports(starterEmployee);
            employee.Employee = starterEmployee;
            employee.NumberOfReports = GetReportCount(starterEmployee, 0);
            return employee;

        }

        public int GetReportCount(Employee employee, int intCount)
        {
            if (employee != null)
            {
                if (employee.DirectReports != null)
                {
                    foreach (var directReport in employee.DirectReports)
                    {
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
            return _compensationContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
    }
}
