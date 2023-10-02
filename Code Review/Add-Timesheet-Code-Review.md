Objective 1 - Code Review

a) Add Timesheet
The flow of the submission process is instigated in the UI via the Razor inserts injected into the index page.

The POST action method 'Index' is invoked on controller TimesheetController in response to the submission of the timesheet form.

index.cshtml
@using (Html.BeginForm("Index", "Timesheet", FormMethod.Post))

My initial observation is that there is no attempt to validate the inputs.

The form can therefore be submitted with missing or incorrect data, which may result in the SaveChanges db method throwing an exception.

One possible workaround for this would be to employ javascript to dynamically deactivate the Submit button unless all conditions are met (i.e. all necessary fields are populated and in the expected format).
I have experience appling such an approach using a combination of Javascript and Angular as part of my prior role working on a SaaS project.

NOTE - A basic example will be staged for site.js, preventing the button being clickable unless both name fields are populated.

Potential Improvements
1) Switch out the manual date field for a date picker widget to help prevent illegal dates from being captured
2) Remove the first/last name fields and implement a login flow. These are error prone and in all likelyhood unnecessary if their details are retrievable via database query based on supplied login credentials. Adding an employee login stage using an established authentication flow that returns the employee's details will improve the experience and help to elleviate errors.
3) Convert and pre-populate the project field as a drop down of known project types to help avoid mistypings.
4) Prevent the number of hours submitted from exceeding 24. This could take the form of a dropdown list of times staged at 15/30 minute intervals or a custom widget.

Given that the UI form is likely only one means to submit timesheet data, it would be recommended to add in process validation to prevent invalid timesheets from being submitted.

TimesheetController.cs
[HttpPost]
public ActionResult Index(TimesheetEntry timesheetEntry)
{
    var timesheet = new Timesheet()
    {
        TimesheetEntry = timesheetEntry,
        TotalHours = timesheetEntry.Hours
    };

    _timesheetService.Add(timesheet);

   var timesheets = _timesheetService.GetAll();

    return View();
}

NOTE - The call to GetAll is redundant in it's currently incarnation and can therefore safely be removed.

This would be an obvious place to insert the validation code as the controller is the point of entry into the back end business logic. This would prevent invalid requests from being carried over into the service and enable the view to be modified to signify the error/s.

public class Timesheet
{
    [Key]
    public int Id { get; set; }
    public TimesheetEntry TimesheetEntry { get; set; }
    public string TotalHours { get; set; }
}

NOTE that TotalHours of the Timesheet class is mapped directly from the hours submitted in TimesheetEntry which serves no obvious function in it's current incarnation. This could simply be a product of the stripped down test project.

Further more, if TotalHours is intended to be an aggregate of TimesheetEntry Hours, then the property would be need to be a list of type and not a single instance of type.

The Id fields of both Timesheet and TimesheetEntry are not explicitely populated prior to submission, but backfilled by way of the component model Key attribute.

TimesheetService.cs
public void Add(Timesheet timesheet) => _timesheetRepository.AddTimesheet(timesheet);

TimesheetRepository.cs
public void AddTimesheet(Timesheet timesheet)
{
    _context.Timesheets.Add(timesheet);
    _context.SaveChanges();
}


DataContext.cs : DbContext
public DbSet<Timesheet> Timesheets { get; set; }

NOTE - The properties of Hours and Total Hours are held as strings which could lead to issues dependant on how the data is utilised. I would suggest  converting the validated string into an int which will ease the overhead of sorting the timesheets.

NOTE - There is a general lack on inconsistency in defining local variables to store the arguments passed into constructors via dependency injection. For example, TimesheetController stores the timesheet service instance as a private local variable, where as the flow of TimesheetService stores it's local variable as readonly.

Readability is an important consideration for cooperative development. There could be an argument for the use of the Lambda expression for single line functions or those that can be compressed into single line functions.

public IList<Timesheet> GetAllTimesheets()
{
    var timesheets = _context.Timesheets.ToList();
    return timesheets;
}

OR

public IList<Timesheet> GetAllTimesheets() => _context.Timesheets.ToList();

NOTE - The inclusion of comments is debatable. It is my take that as a general rule, comments are to be reserved for places in code where the functionality may not be obvious to anyone reviewing it.

NOTE - There are a couple of places with redundent imports (via using).

NOTE - There is flow in place to accomodate the submission of multiple timesheets against the same employee and project. The inclusion of the TotalHours field would seem to suggest that aggregation is being implied here. I can but speculate.

NOTE - There are no checks in place to prevent a resubmission of a same date entry for the same first/last name. Maybe this is intentional, but seems like an odd choice.

NOTE - There is no included web.config for handling custom errors and redirects. The consequence of this is that any server side errors thrown are made visible in the view which is a security issue.

b) Unit test
There is only one xunit test current defined which appears to be using the Moq open source library.

NOTE - There are a number of redundent imports defined which may have been left in intentionally as a clue to which additional test cases are required.

