using Microsoft.EntityFrameworkCore;
using Timetable.Models;

namespace Timetable.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<TimetableDb> Timetables { get; set; }

        public DbSet<AppointmentDb> Appointments { get; set; }
    }
}
