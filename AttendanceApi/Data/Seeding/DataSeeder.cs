using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AttendanceApi.Data.UnitOfWork;
using AttendanceApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AttendanceApi.Data.Seeding
{
    public static class DataSeeder
    {
        public static async Task SeedDatabase(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            // ✅ Ensure Users Exist Before Employees Are Added
            if (!unitOfWork.UsersExist())
            {
                var usersJson = await File.ReadAllTextAsync("Data/Seeding/users.json");
                var users = JsonSerializer.Deserialize<List<UserSeedModel>>(usersJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                foreach (var userSeed in users)
                {
                    var user = new User
                    {
                        UserName = userSeed.Email, // ✅ Ensure UserName is set to Email
                        Email = userSeed.Email,
                        FirstName = userSeed.FirstName,
                        LastName = userSeed.LastName,
                        Role = userSeed.Role
                    };

                    var result = await userManager.CreateAsync(user, userSeed.Password);

                    // ✅ Log errors if user creation fails
                    if (!result.Succeeded)
                    {
                        Console.WriteLine($"❌ Failed to create user {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        continue; // Skip if user creation fails
                    }
                    else
                    {
                        Console.WriteLine($"✅ User created: {user.Email}");
                    }
                }

                // ✅ Ensure all users are saved before inserting employees
                await unitOfWork.SaveChangesAsync();
            }

            // ✅ Now Insert Employees (AFTER Users Exist)
            if (!unitOfWork.EmployeesExist())
            {
                var employeesJson = await File.ReadAllTextAsync("Data/Seeding/employees.json");
                var employees = JsonSerializer.Deserialize<List<EmployeeSeedModel>>(employeesJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                foreach (var employeeSeed in employees)
                {
                    // ✅ Find the corresponding User by Email
                    var user = await unitOfWork.GetUserByEmailAsync(employeeSeed.Email);
                    if (user == null)
                    {
                        Console.WriteLine($"❌ Skipping Employee {employeeSeed.Email} - User not found.");
                        continue; // Skip if User doesn't exist
                    }

                    // ✅ Find the Manager's Employee Record (if applicable)
                    int? managerId = null;
                    if (!string.IsNullOrEmpty(employeeSeed.ManagerEmail))
                    {
                        var managerUser = await unitOfWork.GetUserByEmailAsync(employeeSeed.ManagerEmail);
                        if (managerUser != null)
                        {
                            var managerEmployee = await unitOfWork.Employees.GetEmployeeByUserIdAsync(managerUser.Id);
                            managerId = managerEmployee?.Id; // ✅ Convert `UserId` to `ManagerId`
                        }
                    }

                    var employee = new Employee
                    {
                        FirstName = employeeSeed.FirstName,
                        LastName = employeeSeed.LastName,
                        Role = employeeSeed.Role,
                        UserId = user.Id, // ✅ Ensure UserId is assigned
                        ManagerId = managerId // ✅ Assign ManagerId (int?)
                    };

                    await unitOfWork.Employees.AddEmployeeAsync(employee);
                }

                await unitOfWork.SaveChangesAsync();
            }
        }
    }

    public class UserSeedModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public RoleType Role { get; set; }
        public string Password { get; set; }
    }

    public class EmployeeSeedModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public RoleType Role { get; set; }
        public string Email { get; set; }
        public string ManagerEmail { get; set; } // ✅ New Field for Manager Relationship
    }
}
