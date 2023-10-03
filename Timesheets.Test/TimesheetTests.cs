using Moq;

using Timesheets.Models;
using Timesheets.Repositories;
using Timesheets.Services;

namespace Timesheets.Test
{
    public class TimesheetTests
    {
        private Timesheet BuildTimesheet(float hours = 7.5f)
        {
            return new Timesheet
            {
                Id = 0,
                TimesheetEntry = new TimesheetEntry
                {
                    Id = 0,
                    Date = "01/09/2023",
                    Project = "Test Project",
                    FirstName = "Test",
                    LastName = "Test",
                    Hours = hours.ToString()
                },
                TotalHours = hours.ToString()
            };
        }

        private TimesheetService GetService {
            get => new TimesheetService(new TimesheetRepository(new Infrastructure.DataContext(null)));
        }

        [Fact]
        public void GivenAValidTimesheet_VerifyAddTransactionWasPerformed()
        {
            // Arrange
            var timesheet = BuildTimesheet();
            var mockRepository = new Mock<ITimesheetRepository>();
            mockRepository.Setup(repo => repo.AddTimesheet(It.IsAny<Timesheet>()));

            var timesheetService = new TimesheetService(mockRepository.Object);

            // Act
            timesheetService.Add(timesheet);

            // Assert
            mockRepository.Verify(repo => repo.AddTimesheet(timesheet), Times.Once);
        }

        [Fact]
        public void GivenAValidTimesheet_VerifyTimesheetWasStored()
        {
            // Arrange
            var timesheet = BuildTimesheet();
            var timesheetService = GetService;

            // Act
            timesheetService.Add(timesheet);

            // Assert
            Equals(timesheetService.GetAll().Count, 1);
        }

        [Fact]
        public void GivenAValidTimesheet_VerifyTimesheetStoredProperties()
        {
            // Arrange
            var timesheet = BuildTimesheet();
            var timesheetService = GetService;

            // Act
            timesheetService.Add(timesheet);
            var timesheetEntry = timesheet.TimesheetEntry;

            // Assert
            var dbTimesheeteEntry = timesheetService.GetAll()[0].TimesheetEntry;
            
            Equals(dbTimesheeteEntry.Date.Equals(timesheetEntry.Date) &&
                dbTimesheeteEntry.FirstName.Equals(timesheetEntry.FirstName) &&
                dbTimesheeteEntry.LastName.Equals(timesheetEntry.LastName) &&
                dbTimesheeteEntry.Project.Equals(timesheetEntry.Project) &&
                dbTimesheeteEntry.Hours.Equals(timesheetEntry.Hours),
                true);
        }

        [Fact]
        public void GivenAValidTimesheet_VerifyTimesheetIdAsNonZeroPostSubmission()
        {
            // Arrange
            var timesheet = BuildTimesheet();
            var timesheetService = GetService;

            // Act
            timesheetService.Add(timesheet);

            // Assert
            Equals(timesheetService.GetAll()[0].Id != 0, true);
        }

        [Fact]
        public void GivenAValidTimesheet_VerifyTimesheetIdPostSubmission()
        {
            // Arrange
            var timesheet = BuildTimesheet();
            var timesheetService = GetService;

            // Act
            timesheetService.Add(timesheet);

            // Assert
            Equals(timesheetService.GetAll()[0].Id, 1);
        }

        [Fact]
        public void GivenASetOfValidTimesheets_VerifyTimesheetSummaryDescendingOrdering()
        {
            // Arrange
            var timesheetA = BuildTimesheet(9f);
            var timesheetB = BuildTimesheet(12f);
            var timesheetC = BuildTimesheet(2.5f);
            var timesheetService = GetService;

            // Act
            timesheetService.Add(timesheetA);
            timesheetService.Add(timesheetB);
            timesheetService.Add(timesheetC);

            // Assert
            int lastHours = int.MaxValue;
            bool isDescendng = true;

            foreach(var dbTimesheet in timesheetService.SortTimesheetsByHoursDescending(timesheetService.GetAll()))
            {
                if (float.Parse(dbTimesheet.TimesheetEntry.Hours) > lastHours)
                {
                    isDescendng = false;
                }
            }

            Equals(isDescendng, true);
        }

        [Fact]
        public void GivenAValidTimesheet_VerifyGeneratedCSVLineCountAgainstDb()
        {
            var timesheet = BuildTimesheet();
            var timesheetService = GetService;

            // Act
            timesheetService.Add(timesheet);

            // Assert
            var lineCount = timesheetService.FormatTimesheetDataAsCSV().Split('\n').Length;
            Equals(lineCount - 1, timesheetService.GetAll().Count);
        }
    }
}
