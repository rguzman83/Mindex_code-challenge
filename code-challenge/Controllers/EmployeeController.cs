using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using challenge.Services;
using challenge.Models;

namespace challenge.Controllers
{
    [Route("api/employee")]
    public class EmployeeController : Controller
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpGet("reporting/{id}", Name = "getEmployeeReportingStructure")]
        public IActionResult GetEmployeeReportingStructure(String id)
        {
            _logger.LogDebug($"Received report structure get request for '{id}'");
            var employeeWithStruct = _employeeService.GetReportStructure(id);

            //Console.WriteLine("ID pinged - " + id);
            if (employeeWithStruct.Employee == null)
                return NotFound();
            return Ok(employeeWithStruct);
        }

        [HttpGet("compensation/{id}", Name = "getEmployeeCompensation")]
        public IActionResult GetEmployeeCompensation(String id)
        {
            _logger.LogDebug($"Recieved compensation request for '{id}'");

            var employeeCompensation = _employeeService.GetEmployeeCompensation(id);
            if(employeeCompensation == null)
                return NotFound();
            return Ok(employeeCompensation);
        }

        [HttpPost("compensation/{id}")]
        public IActionResult CreateCompensation([FromBody] Compensation employee)
        {
            _logger.LogDebug($"Recieved compensation request for '{employee.EmployeeID}'");

            _employeeService.AddEmployeeCompensation(employee);
            return CreatedAtRoute("getEmployeeCompensation", new {id = employee.EmployeeID}, employee);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }
    }
}
