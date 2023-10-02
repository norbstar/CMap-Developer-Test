using Microsoft.EntityFrameworkCore;

using Timesheets.Models;

namespace Timesheets.Infrastructure
{
    public class DataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration) => Configuration = configuration;

        // An in memory database used for simplicity, change to a real db for production applications
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseInMemoryDatabase("TimesheetDB");

        public DbSet<Timesheet> Timesheets { get; set; }

        public DbSet<TimesheetEntry> TimesheetsEntries { get; set; }
    }
}
