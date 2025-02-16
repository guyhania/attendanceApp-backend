using AttendanceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AttendanceApi.Data.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttendanceReport>> GetAllReportsAsync()
        {
            return await _context.AttendanceReports.ToListAsync();
        }
        public async Task<List<AttendanceReport>> GetReportsByEmployeeIdsAsync(List<int> employeeIds)
        {
            return await _context.AttendanceReports.Where(r => employeeIds.Contains(r.EmployeeId)).ToListAsync();
        }

        public async Task<AttendanceReport> GetReportByIdAsync(int id)
        {
            return await _context.AttendanceReports.FindAsync(id);
        }

        public async Task AddReportAsync(AttendanceReport report)
        {
            await _context.AttendanceReports.AddAsync(report);
        }

        public async Task UpdateReportAsync(AttendanceReport report)
        {
            _context.AttendanceReports.Update(report);
        }

        public async Task DeleteReportAsync(int id)
        {
            var report = await GetReportByIdAsync(id);
            if (report != null)
            {
                _context.AttendanceReports.Remove(report);
            }
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.AttendanceReports.AnyAsync(e => e.Id == id);
        }
        public async Task<bool> HasReportForTodayAsync(int employeeId)
        {
            var today = DateTime.UtcNow.Date.Date;
            return await _context.AttendanceReports.AnyAsync(r => r.EmployeeId == employeeId && r.Date == today);
        }
    }
}
