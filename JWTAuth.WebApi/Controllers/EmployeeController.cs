using JWTAuth.WebApi.Interfaces;
using JWTAuth.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JWTAuth.WebApi.Controllers
{
    [Authorize]
    [Route("api/employee")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        // GET: api/employee>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> Get()
        {
            return await Task.FromResult(employeeService.GetEmployeeDetails());
        }

        // GET api/employee/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> Get(int id)
        {
            var employees = await Task.FromResult(employeeService.GetEmployeeDetails(id));
            if (employees == null)
            {
                return NotFound();
            }
            return employees;
        }

        // POST api/employee
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<Employee>> Post(Employee employee)
        {
            employeeService.AddEmployee(employee);
            //return await Task.FromResult(CreatedAtAction("GetEmployees", new { id = employee.EmployeeID }, employee));
            return await Task.FromResult(employee);
        }

        // PUT api/employee/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Employee>> Put(int id, Employee employee)
        {
            if (id != employee.EmployeeID)
            {
                return BadRequest();
            }
            try
            {
                employeeService.UpdateEmployee(employee);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return await Task.FromResult(employee);
        }

        // DELETE api/employee/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Employee>> Delete(int id)
        {
            var employee = employeeService.DeleteEmployee(id);
            return await Task.FromResult(employee);
        }

        private bool EmployeeExists(int id)
        {
            return employeeService.CheckEmployee(id);
        }
    }
}
