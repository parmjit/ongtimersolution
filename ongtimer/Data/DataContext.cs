using Microsoft.EntityFrameworkCore;
using ongtimer.Models;

namespace ongtimer.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> context):base(context) { }

        public DbSet<Life> Lives { get; set; }
        public DbSet<UserRecord> UserRecords { get; set; }

    }
}
