using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AttendanceApi.Data;
using AttendanceApi.Models;
using AttendanceApi.Data.UnitOfWork;

namespace AttendanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _unitOfWork.Employees.GetAllEmployeesAsync();
            return Ok(employees);
        }
        
        // GET: api/Employees/Managers
        [HttpGet("managers")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetManagers()
        {
            var managers = await _unitOfWork.Employees.GetManagersAsync();
            return Ok(managers);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _unitOfWork.Employees.GetEmployeeByIdAsync(id);
            if (employee == null) return NotFound();

            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _unitOfWork.Employees.AddEmployeeAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
        }
    }
}
