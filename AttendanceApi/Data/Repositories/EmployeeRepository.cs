using AttendanceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AttendanceApi.Data.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;
        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees.ToListAsync();
            //    return await _context.Employees.Include(e => e.Manager).ToListAsync();

        }
        public async Task<IEnumerable<Employee>> GetManagersAsync()
        {
            return await _context.Employees.Where(e => e.Role == RoleType.Manager).ToListAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees.Include(e => e.Manager)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
        //public async Task<Employee?> GetEmployeeByUserNameAsync(string userName)
        //{
        //    return await _context.Employees.FirstOrDefaultAsync(e => e.Email == userName);
        //}
        public async Task<Employee> GetEmployeeByUserIdAsync(string userId)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByManagerIdAsync(int managerId)
        {
            return await _context.Employees.Where(e => e.ManagerId == managerId).ToListAsync();
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
        }

    }
}
