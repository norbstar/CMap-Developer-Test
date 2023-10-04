using System.Globalization;
using System.Text;

using Timesheets.Models;
using Timesheets.Repositories;

namespace Timesheets.Services
{
    public interface ITimesheetService
    {
        void ValidateTimesheet(Timesheet timesheet);
        void Add(Timesheet timesheet);
        IList<Timesheet> GetAll();
        List<Timesheet> SortTimesheetsByHoursDescending(IList<Timesheet> timesheets);
        string FormatTimesheetDataAsCSV();
    }

    public class TimesheetService : ITimesheetService
    {
        private readonly ITimesheetRepository _timesheetRepository;

        public TimesheetService(ITimesheetRepository timesheetRepository) => _timesheetRepository = timesheetRepository;

        public void ValidateTimesheet(Timesheet timesheet)
        {
            if (timesheet == null)
            {
                throw new ArgumentNullException();
            }

            if (timesheet.TimesheetEntry.Date == null || !DateTime.TryParseExact(timesheet.TimesheetEntry.Date, new string[] { "dd/MM/yyyy", "MM/dd/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                throw new ArgumentException("The Date field is invalid!");
            }

            if (timesheet.TimesheetEntry.FirstName == null || timesheet.TimesheetEntry.FirstName.Length == 0)
            {
                throw new ArgumentException("The First Name field is invalid!");
            }

            if (timesheet.TimesheetEntry.LastName == null || timesheet.TimesheetEntry.LastName.Length == 0)
            {
                throw new ArgumentException("The Last Name field is invalid!");
            }

            if (timesheet.TimesheetEntry.Project == null || timesheet.TimesheetEntry.Project.Length == 0)
            {
                throw new ArgumentException("The Project field is invalid!");
            }

            if (timesheet.TimesheetEntry.Hours == null || float.Parse(timesheet.TimesheetEntry.Hours) > 24)
            {
                throw new ArgumentException("The Hours field is invalid!");
            }
        }

        public void Add(Timesheet timesheet)
        {
            ValidateTimesheet(timesheet);
            _timesheetRepository.AddTimesheet(timesheet);
        }

        public IList<Timesheet> GetAll() => _timesheetRepository.GetAllTimesheets();

        public List<Timesheet> SortTimesheetsByHoursDescending(IList<Timesheet> timesheets) => timesheets.OrderByDescending(t => float.Parse(t.TimesheetEntry.Hours)).ToList();

        public string FormatTimesheetDataAsCSV()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Id,FirstName,LastName,Project,Hours");

            foreach (var timesheet in GetAll())
            {
                sb.AppendLine(timesheet.Id + "," + timesheet.TimesheetEntry.FirstName + "," + timesheet.TimesheetEntry.LastName + "," + timesheet.TimesheetEntry.Project + "," + timesheet.TimesheetEntry.Hours);
            }

            return sb.ToString();
        }
    }
}
