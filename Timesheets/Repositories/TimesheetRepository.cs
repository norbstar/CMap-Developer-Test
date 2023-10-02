using Timesheets.Infrastructure;
using Timesheets.Models;

namespace Timesheets.Repositories
{
    public interface ITimesheetRepository
    {
        void AddTimesheet(Timesheet timesheet);
        IList<Timesheet> GetAllTimesheets();
    }

    public class TimesheetRepository : ITimesheetRepository
    {
        private readonly DataContext _context;

        public TimesheetRepository(DataContext context) => _context = context;

        public void AddTimesheet(Timesheet timesheet)
        {
            _context.Timesheets.Add(timesheet);
            _context.TimesheetsEntries.Add(timesheet.TimesheetEntry);
            _context.SaveChanges();
        }

        public IList<Timesheet> GetAllTimesheets()
        {
            var entries = _context.TimesheetsEntries.ToList();
            return _context.Timesheets.ToList();
        }
    }
}
