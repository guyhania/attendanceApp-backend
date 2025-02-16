using AttendanceApi.Models;

namespace AttendanceApi.Data.Repositories
{
    public interface IReportRepository
    {
        Task<IEnumerable<AttendanceReport>> GetAllReportsAsync();
        Task<AttendanceReport> GetReportByIdAsync(int id);
        Task<List<AttendanceReport>> GetReportsByEmployeeIdsAsync(List<int> employeeIds);
        Task AddReportAsync(AttendanceReport report);
        Task UpdateReportAsync(AttendanceReport report);
        Task<bool> ExistsAsync(int id);
        Task<bool> HasReportForTodayAsync(int employeeId);

        Task DeleteReportAsync(int id);
    }
}
