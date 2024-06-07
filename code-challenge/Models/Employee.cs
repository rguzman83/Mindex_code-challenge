using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace challenge.Models
{
    public class Employee
    {
        public String EmployeeId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Position { get; set; }
        public String Department { get; set; }
        public List<Employee> DirectReports { get; set; }
    }

    public class ReportingStructure
    { 
        public int NumberOfReports { get; set; }
        public Employee Employee { get; set; }
    }

    public class Compensation
    {
        public Employee Employee { get; set; }
        public double Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
