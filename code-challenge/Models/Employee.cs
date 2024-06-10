using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

    public class CompensationPost // Class for POST Format
    {
        public string EmployeeID { get; set; }
        public double Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
    }

    public class Compensation // Class for return data
    {
        [JsonIgnore] //force-creating a primary key that I want to have not output in the JSON return.
        public string CompensationID { get; set; }  
        public Employee Employee { get; set; } // Would prefer to get Primary/Foreign key from here
        public double Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
