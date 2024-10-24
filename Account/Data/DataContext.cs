using Account.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Account.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<SignUp> SignUps { get; set; }
        public DbSet<SignIn> SignIns { get; set; }
        public DbSet<AccountDb> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SignUp>().HasData(
                new AccountDb { Id = 1, LastName = "Null", FirstName = "Null", Username = "admin", Password = "admin" },
                new AccountDb { Id = 2, LastName = "Null", FirstName = "Null", Username = "manager", Password = "manager" },
                new AccountDb { Id = 3, LastName = "Null", FirstName = "Null", Username = "doctor", Password = "doctor" },
                new AccountDb { Id = 4, LastName = "Null", FirstName = "Null", Username = "user", Password = "user" }
            );

            modelBuilder.Entity<AccountDb>().HasData(
                new AccountDb { Id = 1, LastName = "Null", FirstName = "Null", Username = "admin", Password = "admin", Roles = new[] { "Admin" } },
                new AccountDb { Id = 2, LastName = "Null", FirstName = "Null", Username = "manager", Password = "manager", Roles = new[] { "Manager" } },
                new AccountDb { Id = 3, LastName = "Null", FirstName = "Null", Username = "doctor", Password = "doctor", Roles = new[] { "Doctor" } },
                new AccountDb { Id = 4, LastName = "Null", FirstName = "Null", Username = "user", Password = "user", Roles = new[] { "User" } }
            );
        }
    }
}