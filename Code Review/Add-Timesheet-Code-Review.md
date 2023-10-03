# OBJECTIVE ONE - CODE REVIEW

## ADD TIMESHEETS

The flow of the submission process is instigated in the UI via the Razor inserts injected into the index page.

The POST action method 'Index' is invoked on controller TimesheetController in response to the submission of the timesheet form.

index.cshtml
@using (Html.BeginForm("Index", "Timesheet", FormMethod.Post))

My initial observation is that there is no attempt to validate the inputs.

The form can therefore be submitted with missing or incorrect data, which may result in the SaveChanges db method throwing an exception.

One possible workaround for this would be to employ javascript to dynamically deactivate the Submit button unless all conditions are met (i.e. all necessary fields are populated and in the expected format).
I have experience appling such an approach using a combination of Javascript and Angular as part of my prior role working on a SaaS project.

**NOTE** - A basic example will be staged for site.js, preventing the button being clickable unless both name fields are populated.

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

**OBSERVATION** - The call to GetAll is redundant in it's currently incarnation and can therefore safely be removed.

This would be an obvious place to insert the validation code as the controller is the point of entry into the back end business logic. This would prevent invalid requests from being carried over into the service and enable the view to be modified to signify the error/s.

public class Timesheet
{
    [Key]
    public int Id { get; set; }
    public TimesheetEntry TimesheetEntry { get; set; }
    public string TotalHours { get; set; }
}

**OBSERVATION** - TotalHours of the Timesheet class is mapped directly from the hours submitted in TimesheetEntry which serves no obvious function in it's current incarnation. This could simply be a product of the stripped down test project.
Further more, if TotalHours is intended to be an aggregate of TimesheetEntry Hours, then the property would be need to be a list of type and not a single instance of type.

**OBSERVATION** - The Id fields of both Timesheet and TimesheetEntry are not explicitely populated prior to submission, but backfilled by way of the component model Key attribute.

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

**OBSERVATION** - The properties of Hours and Total Hours are held as strings which could lead to issues dependant on how the data is utilised. I would suggest  converting the validated string into an int which will ease the overhead of sorting the timesheets.

**OBSERVATION** - There is a general lack on inconsistency in defining local variables to store the arguments passed into constructors via dependency injection. For example, TimesheetController stores the timesheet service instance as a private local variable, where as the flow of TimesheetService stores it's local variable as readonly.

**OBSERVATION** - Readability is an important consideration for cooperative development. There could be an argument for the use of the Lambda expression for single line functions or those that can be compressed into single line functions.

public IList<Timesheet> GetAllTimesheets()
{
    var timesheets = _context.Timesheets.ToList();
    return timesheets;
}

OR

public IList<Timesheet> GetAllTimesheets() => _context.Timesheets.ToList();

**OBSERVATION** - The inclusion of comments is debatable. It is my take that as a general rule, comments are to be reserved for places in code where the functionality may not be obvious to anyone reviewing it.

**OBSERVATION** - There are a couple of places with redundent imports (via using).

**OBSERVATION** - There is no flow in place to accomodate the submission of multiple timesheets against the same employee and project.
The inclusion of the TotalHours field would seem to suggest that aggregation is being implied here. I can but speculate.

**OBSERVATION** - There are no checks in place to prevent a resubmission of a same date entry for the same first/last name.
Maybe this is intentional, but seems like an odd choice.

**OBSERVATION** - There is no feedback mechanism in place to cater for successes and failures in the processing of actions.

**OBSERVATION** - There is no included web.config for handling custom errors and redirects.
The consequence of this is that any server side errors thrown are made visible in the view which is a security issue.

## END ADD TIMESHEETS

## UNIT TESTS

There is only one xunit test current defined which appears to be using the Moq open source library.

It verifies that a singular invocation matching the given expression was performed on the mock.
In this case it is just some instance of Timesheet (It.IsAny<Timesheet>()), the specific properties of which are irrelevant.

**OBSERVATION** - There are a number of redundent imports defined which may have been left in intentionally as a clue to which additional test cases are required.

GivenAValidTimesheet_VerifyAddTransactionWasPerformed
Ensures that a transaction that takes a Timesheet reference was performed.

**NOTE** - This orignal xunit text name change as it does not accurately describe the test as no data is actually stored

GivenAValidTimesheet_VerifyTimesheetWasStored
Will ensure that a Timesheet instance was successfullt stored in the database

GivenAValidTimesheet_VerifyTimesheetStoredProperties
Will ensure that the properties supplied for a valid timesheet are actually being stored as those properties in the database.

GivenAValidTimesheet_VerifyTimesheetIdAsNonZeroPostSubmission
Will ensure that the id field of a submitted timesheet is being updated to some non zero integer value in response to the [Key] attribution under control of the database.

GivenAValidTimesheet_VerifyTimesheetIdPostSubmission
Will ensure that the id field of a submitted timesheet is being updated to a specific sequential integer value in response to the [Key] attribution under control of the database.
This would entail adding one or more timesheets as part of the same test and verifying that the id of the last entry corresponds to that expected based on sequential allocation.
i.e. the third entry id would be 3

GivenASetOfValidTimesheets_VerifyTimesheetSummaryDescendingOrdering
Will ensure that the generated timesheet summary output is in descending order of hours

GivenAValidTimesheet_VerifyGeneratedCSVLineCountAgainstDb
Will ensure that if the sorted list size differs from the original list size, it will fail

**OBSERVATION** - If the validation routines were in place to prevent an invalid timesheet from being submitted at the controller level and a suitable mechanism were put in place to provide feedback,
a set of unit tests would be required to verify failure of various scenarios.

An invalid date (GivenAnInvalidTimesheetDate_VerifyFailureToSubmit)
An undefined project type (GivenAnInvalidTimesheetProject_VerifyFailureToSubmit)
An invalid number of hours (GivenAnInvalidTimesheetHours_VerifyFailureToSubmit)
An unknown employee id (GivenAnInvalidTimesheetEmployeeId_VerifyFailureToSubmit)

**NOTE** As these do not constitute explicit requirements of the ReadMe.md they will not be implemented in controller logic or in the form of xunit tests.
They are simple mentioned here as an observation as part of more rounded implementation.

## END UNIT TESTS