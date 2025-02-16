using AttendanceApi.Data.Repositories;
using AttendanceApi.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace AttendanceApi.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        IReportRepository AttendanceReports { get; }
        IEmployeeRepository Employees { get; }
        Task<int> SaveChangesAsync();

        // Seeding Helper Methods
        bool UsersExist();
        bool EmployeesExist();
        Task<User> GetUserByEmailAsync(string email);
    }
}
