using AttendanceApi.Data.Repositories;
using AttendanceApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AttendanceApi.Data.UnitOfWork
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            AttendanceReports = new ReportRepository(context);
            Employees = new EmployeeRepository(context);
        }

        public IReportRepository AttendanceReports { get; }
        public IEmployeeRepository Employees { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
     
        public bool UsersExist()
        {
            return _context.Users.Any();
        }

        public bool EmployeesExist()
        {
            return _context.Employees.Any();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
