using AttendanceApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection.Emit;


namespace AttendanceApi.Data
{
    public class AppDbContext: IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<AttendanceReport> AttendanceReports { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //var seedData = LoadSeedData();
            //if (seedData != null)
            //{
            //    builder.Entity<Employee>().HasData(seedData);
            //}
            base.OnModelCreating(builder);
            
            // Ensure User-Employee relationship
            builder.Entity<Employee>()
                   .HasOne(e => e.User)
                   .WithMany()
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
        //private List<Employee> LoadSeedData()
        //{
        //    try
        //    {
        //        var jsonData = File.ReadAllText("seed-data.json");
        //        var data = JsonConvert.DeserializeObject<SeedData>(jsonData);
        //        return data?.Employees ?? new List<Employee>();
        //    }
        //    catch
        //    {
        //        return new List<Employee>();
        //    }
        //}
    }
}
//public class SeedData
//{
//    public List<Employee>? Employees { get; set; }
//}