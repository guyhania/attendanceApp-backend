using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AttendanceApi.Models
{
    public enum RoleType
    {
        Employee,
        Manager
    }
    public class User: IdentityUser
    {
        [MaxLength(50)]
        public required string FirstName { get; set; }

        [MaxLength(50)]
        public required string LastName { get; set; }

        public required RoleType Role { get; set; }

        public int ManagerId { get; set; }
        
    }
}
