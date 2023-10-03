using Microsoft.AspNetCore.Mvc;
using System.Text;
using Timesheets.Models;
using Timesheets.Repositories;

namespace Timesheets.Services
{
    public interface ITimesheetService
    {
        void Add(Timesheet timesheet);
        IList<Timesheet> GetAll();
        List<Timesheet> SortTimesheetsByHoursDescending(IList<Timesheet> timesheets);
        string FormatTimesheetDataAsCSV();
    }

    public class TimesheetService : ITimesheetService
    {
        private readonly ITimesheetRepository _timesheetRepository;

        public TimesheetService(ITimesheetRepository timesheetRepository) => _timesheetRepository = timesheetRepository;

        public void Add(Timesheet timesheet) => _timesheetRepository.AddTimesheet(timesheet);

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
