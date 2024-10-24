using Document.Models;
using Microsoft.EntityFrameworkCore;

namespace Document.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<DocumentDb> Documents { get; set; }
    }
}
