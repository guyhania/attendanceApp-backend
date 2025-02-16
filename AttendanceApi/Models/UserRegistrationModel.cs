using System.ComponentModel.DataAnnotations;

namespace AttendanceApi.Models
{
    public class UserRegistrationModel
    {
        [MaxLength(50)]
        public required string FirstName { get; set; }

        [MaxLength(50)]
        public required string LastName { get; set; }

        public required string Role { get; set; }

        public int ManagerId { get; set; }

        [EmailAddress]
        public required string Email { get; set; }      

        [MinLength(6)]
        public required string Password { get; set; }
    }
}
