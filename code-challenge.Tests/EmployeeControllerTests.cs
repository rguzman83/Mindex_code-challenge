using challenge.Controllers;
using challenge.Data;
using challenge.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using code_challenge.Tests.Integration.Extensions;

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using code_challenge.Tests.Integration.Helpers;
using System.Text;

namespace code_challenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<TestServerStartup>()
                .UseEnvironment("Development"));

            _httpClient = _testServer.CreateClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void GetReportCountById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedCount = 4;

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/reporting/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var reporting = response.DeserializeContent<ReportingStructure>();
            Assert.AreEqual(expectedCount, reporting.NumberOfReports);
        }

        [TestMethod]
        public void AddEmployeeCompensation_Returns_Created()
        {
            // Arrange

            var compensationRecord = new CompensationPost()
            {
                EmployeeID = "16a596ae-edd3-4847-99fe-c4518e82c86f",
                Salary = 50.00,
                EffectiveDate = DateTime.Parse("2024-06-02T00:00:00Z")
            };
            var requestContent = new JsonSerialization().ToJson(compensationRecord);

            // Execute
            var postRequestTask = _httpClient.PostAsync($"api/employee/compensation/{compensationRecord.EmployeeID}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var postResponse = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode);
            var newCompensationRecord = postResponse.DeserializeContent<CompensationPost>();

            Assert.AreEqual(newCompensationRecord.EmployeeID, compensationRecord.EmployeeID);
            Assert.AreEqual(newCompensationRecord.Salary, compensationRecord.Salary);
        }


        [TestMethod]
        public void GetEmployeeCompensationById_Returns_OK()
        {
            // Arrange
            var expectedFirstName = "Paul";
            var expectedLastName = "McCartney";

            var compensationRecord = new CompensationPost()
            {
                EmployeeID = "b7839309-3348-463b-a7e3-5de1c168beb3",
                Salary = 50.00,
                EffectiveDate = DateTime.Parse("2024-06-02T00:00:00Z")
            };
            var requestContent = new JsonSerialization().ToJson(compensationRecord);

            // Execute
            var postRequestTask = _httpClient.PostAsync($"api/employee/compensation/{compensationRecord.EmployeeID}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var getRequestTask = _httpClient.GetAsync($"api/employee/compensation/{compensationRecord.EmployeeID}");

            var getResponse = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);
            var newCompensationRecord = getResponse.DeserializeContent<Compensation>();

            Assert.AreEqual(newCompensationRecord.Employee.EmployeeId, compensationRecord.EmployeeID);
            Assert.AreEqual(newCompensationRecord.Employee.FirstName, expectedFirstName);
            Assert.AreEqual(newCompensationRecord.Employee.LastName, expectedLastName);
            Assert.AreEqual(newCompensationRecord.Salary, compensationRecord.Salary);
        }
    }
}

