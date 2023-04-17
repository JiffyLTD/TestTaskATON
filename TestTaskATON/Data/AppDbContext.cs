using Microsoft.EntityFrameworkCore;
using TestTaskATON.Models;

namespace TestTaskATON.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();

            if(Users.ToList().Count < 1)
            {
                User user = new("admin", "admin", "AdminName", 2, DateTime.Now, true, "admin");

                Users.Add(user);
                SaveChanges();
            }
        }

        public DbSet<User> Users { get; set; }
    }
}
