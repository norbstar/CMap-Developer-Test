using System.ComponentModel.DataAnnotations;

namespace Timesheets.Models
{
    public class Timesheet
    {
        [Key]
        public int Id { get; set; }
        public TimesheetEntry TimesheetEntry { get; set; }
        public string TotalHours { get; set; }
    }
}
