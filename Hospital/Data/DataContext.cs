using Hospital.Models;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<HospitalDb> Hospitals { get; set; }
    }
}
