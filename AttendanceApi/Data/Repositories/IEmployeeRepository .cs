using AttendanceApi.Models;

namespace AttendanceApi.Data.Repositories
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<IEnumerable<Employee>> GetManagersAsync();
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task<Employee> GetEmployeeByUserIdAsync(string userId);
        //Task<Employee> GetEmployeeByUserNameAsync(string userName);

        Task<IEnumerable<Employee>> GetEmployeesByManagerIdAsync(int managerId);
        Task AddEmployeeAsync(Employee employee);
    }
}
