using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceApi.Models
{
    public class AttendanceReport
    {
        [Key] // EF automatically sets this as the primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public required string EmployeeFullName { get; set; }
        public required DateTime Date { get; set; }
        public TimeSpan? StartTime  { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? StartTimeText { get; set; }
        public string? EndTimeText { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
