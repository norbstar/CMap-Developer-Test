using Microsoft.AspNetCore.Mvc;

using Timesheets.Models;
using Timesheets.Services;

namespace Timesheets.Controllers
{
    public class TimesheetViewerController : Controller
    {
        private readonly ITimesheetService _timesheetService;

        public TimesheetViewerController(ITimesheetService timesheetService) => _timesheetService = timesheetService;

        private List<Timesheet> SortTimesheetsByHoursDescending(IList<Timesheet> timesheets) => timesheets.OrderByDescending(t => float.Parse(t.TimesheetEntry.Hours)).ToList();

        public IActionResult Index() => View(SortTimesheetsByHoursDescending(_timesheetService.GetAll()));
    }
}
