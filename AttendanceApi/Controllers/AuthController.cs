using AttendanceApi.Data;
using AttendanceApi.Data.UnitOfWork;
using AttendanceApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AttendanceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;


        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(model.Email);
            bool isReportExists = false;
            if (user == null || !(await _userManager.CheckPasswordAsync(user, model.Password)))
                return Unauthorized(new { message = "Invalid credentials" });

            // Retrieve Employee Details
            var employee = await _unitOfWork.Employees.GetEmployeeByUserIdAsync(user.Id);
            if (employee == null)
                return Unauthorized(new { message = "User is not linked to an Employee record." });

            var token = GenerateJwtToken(user);
            List<AttendanceReport> attendanceReports = new List<AttendanceReport>();
            if (user.Role == RoleType.Manager)
            {
                var subordinates = await _unitOfWork.Employees.GetEmployeesByManagerIdAsync(employee.Id);
                var subordinateIds = subordinates.Select(e => e.Id).ToList();
                attendanceReports = await _unitOfWork.AttendanceReports.GetReportsByEmployeeIdsAsync(subordinateIds);
            }
            if (user.ManagerId != 0)
            {
                isReportExists = await _unitOfWork.AttendanceReports.HasReportForTodayAsync(employee.Id);
            }
            return Ok(new
            {
                token,
                employee = new
                {
                    employee.Id,
                    employee.FirstName,
                    employee.LastName,
                    employee.Role,
                    employee.ManagerId,
                    employee.UserId
                },
                attendanceReports,
                isReportExists
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Role = model.Role == "Manager" ? RoleType.Manager : RoleType.Employee,
                ManagerId = model.ManagerId
            };

            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                var employee = new Employee
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Role = model.Role == "Manager" ? RoleType.Manager : RoleType.Employee,
                    ManagerId = model.ManagerId,
                    UserId = user.Id
                };

                await _unitOfWork.Employees.AddEmployeeAsync(employee);
                await _unitOfWork.SaveChangesAsync();

                return Ok(new { message = "User registered successfully, Employee record created." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error during registration: {ex.Message}");
                return StatusCode(500, "Internal Server Error. Could not complete registration.");
            }
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var signingKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddHours(1),
                claims: claims,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
