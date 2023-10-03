using System.Diagnostics;
using System.Text;

using Microsoft.AspNetCore.Mvc;

using Timesheets.Models;
using Timesheets.Services;

namespace Timesheets.Controllers
{
    public class TimesheetController : Controller
    {
        private readonly ITimesheetService _timesheetService;

        public TimesheetController(ITimesheetService timesheetService) => _timesheetService = timesheetService;

        public IActionResult Index() => View();

        [HttpPost]
        public ActionResult Index(TimesheetEntry timesheetEntry)
        {
            var timesheet = new Timesheet()
            {
                TimesheetEntry = timesheetEntry,
                TotalHours = timesheetEntry.Hours
            };

            _timesheetService.Add(timesheet);

            return View();
        }

        public IActionResult ExportTimesheetsToCSVFile()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Id,FirstName,LastName,Project,Hours");

            foreach (var timesheet in _timesheetService.GetAll())
            {
                sb.AppendLine(timesheet.Id + "," + timesheet.TimesheetEntry.FirstName + "," + timesheet.TimesheetEntry.LastName + "," + timesheet.TimesheetEntry.Project + "," + timesheet.TimesheetEntry.Hours);
            }

            return File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv", "Timesheets.csv");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}