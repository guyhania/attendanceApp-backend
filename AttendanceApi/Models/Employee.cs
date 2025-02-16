using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AttendanceApi.Models
{
    public class Employee
    {
        [Key] // EF automatically sets this as the primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment in the DB
        public int Id { get; set; } // EF will handle this automatically
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required RoleType Role { get; set; }

        public int? ManagerId { get; set; }
        
        [JsonIgnore] // Prevents circular reference
        public Employee? Manager { get; set; }

        [JsonIgnore] // Prevents circular reference

        public ICollection<Employee> Subordinates { get; set; } = new List<Employee>();

        public required string UserId { get; set; } // Foreign Key to Identity User Table

        [ForeignKey("UserId")]
        public User User { get; set; } // Navigation Property
    }
}
