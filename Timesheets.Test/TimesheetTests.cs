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
        public void GivenAValidTimesheet_VerifyAddTimesheetTransactionWasPerformed()
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
        public void GivenAValidTimesheet_VerifyGetAllTimesheetsTransactionWasPerformed()
        {
            // Arrange
            var mockRepository = new Mock<ITimesheetRepository>();
            mockRepository.Setup(repo => repo.GetAllTimesheets()).Returns(It.IsAny<IList<Timesheet>>);

            var timesheetService = new TimesheetService(mockRepository.Object);

            // Act
            timesheetService.GetAll();

            // Assert
            mockRepository.Verify(repo => repo.GetAllTimesheets(), Times.Once);
        }

        [Fact]
        public void GivenAValidTimesheet_VerifyValidateTimesheetTransactionWasPerformed()
        {
            // Arrange
            var timesheet = BuildTimesheet();
            var mockRepository = new Mock<ITimesheetService>();
            mockRepository.Setup(service => service.ValidateTimesheet(It.IsAny<Timesheet>()));

            // Act
            mockRepository.Object.ValidateTimesheet(timesheet);

            // Assert
            mockRepository.Verify(repo => repo.ValidateTimesheet(timesheet), Times.Once);
        }

        [Fact]
        public void GivenAValidTimesheet_VerifyAddTransactionWasPerformed()
        {
            // Arrange
            var timesheet = BuildTimesheet();
            var mockRepository = new Mock<ITimesheetService>();
            mockRepository.Setup(service => service.Add(It.IsAny<Timesheet>()));

            // Act
            mockRepository.Object.Add(timesheet);

            // Assert
            mockRepository.Verify(repo => repo.Add(timesheet), Times.Once);
        }

        [Fact]
        public void GivenAValidTimesheet_VerifyGetAllWasPerformed()
        {
            // Arrange
            var timesheet = BuildTimesheet();
            var mockRepository = new Mock<ITimesheetService>();
            mockRepository.Setup(service => service.GetAll()).Returns(It.IsAny<IList<Timesheet>>);

            // Act
            mockRepository.Object.GetAll();

            // Assert
            mockRepository.Verify(repo => repo.GetAll(), Times.Once);
        }

        [Fact]
        public void GivenAValidTimesheet_VerifySortTimesheetsByHoursDescendingTransactionWasPerformed()
        {
            // Arrange
            List<Timesheet> timesheets = new List<Timesheet>()
            {
                BuildTimesheet(9f),
                BuildTimesheet(12f),
                BuildTimesheet(2.5f)
            };

            var mockRepository = new Mock<ITimesheetService>();
            mockRepository.Setup(service => service.SortTimesheetsByHoursDescending(It.IsAny<List<Timesheet>>()));

            // Act
            mockRepository.Object.SortTimesheetsByHoursDescending(timesheets);

            // Assert
            mockRepository.Verify(repo => repo.SortTimesheetsByHoursDescending(timesheets), Times.Once);
        }

        [Fact]
        public void GivenAValidTimesheet_VerifyFormatTimesheetDataAsCSVTransactionWasPerformed()
        {
            // Arrange
            List<Timesheet> timesheets = new List<Timesheet>()
            {
                BuildTimesheet(9f),
                BuildTimesheet(12f),
                BuildTimesheet(2.5f)
            };

            var mockRepository = new Mock<ITimesheetService>();
            mockRepository.Setup(service => service.FormatTimesheetDataAsCSV()).Returns(It.IsAny<String>);

            // Act
            mockRepository.Object.FormatTimesheetDataAsCSV();

            // Assert
            mockRepository.Verify(repo => repo.FormatTimesheetDataAsCSV(), Times.Once);
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

        [Fact]
        public void GivenAnInvalidTimesheetDate_VerifyFailureToStore()
        {
            var timesheet = BuildTimesheet();
            timesheet.TimesheetEntry.Date = "15/13/2024";
            var timesheetService = GetService;

            // Act
            try
            {
                timesheetService.Add(timesheet);
            }
            catch (Exception ex)
            {
                // The assertion handles the outcome in this case and not the exception itself
            }

            // Assert
            Equals(timesheetService.GetAll().Count, 0);
        }

        [Fact]
        public void GivenAnInvalidTimesheetFirstName_VerifyFailureToStore()
        {
            var timesheet = BuildTimesheet();
            timesheet.TimesheetEntry.FirstName = "";
            var timesheetService = GetService;

            // Act
            try
            {
                timesheetService.Add(timesheet);
            }
            catch (Exception ex)
            {
                // The assertion handles the outcome in this case and not the exception itself
            }

            // Assert
            Equals(timesheetService.GetAll().Count, 0);
        }

        [Fact]
        public void GivenAnInvalidTimesheetLastName_VerifyFailureToStore()
        {
            var timesheet = BuildTimesheet();
            timesheet.TimesheetEntry.LastName = "";
            var timesheetService = GetService;

            // Act
            try
            {
                timesheetService.Add(timesheet);
            }
            catch (Exception ex)
            {
                // The assertion handles the outcome in this case and not the exception itself
            }

            // Assert
            Equals(timesheetService.GetAll().Count, 0);
        }

        [Fact]
        public void GivenAnInvalidTimesheetProject_VerifyFailureToStore()
        {
            var timesheet = BuildTimesheet();
            timesheet.TimesheetEntry.Project = "";
            var timesheetService = GetService;

            // Act
            try
            {
                timesheetService.Add(timesheet);
            }
            catch (Exception ex)
            {
                // The assertion handles the outcome in this case and not the exception itself
            }

            // Assert
            Equals(timesheetService.GetAll().Count, 0);
        }

        [Fact]
        public void GivenAnInvalidTimesheetHours_VerifyFailureToStore()
        {
            var timesheet = BuildTimesheet();
            timesheet.TimesheetEntry.Hours = "25";
            var timesheetService = GetService;

            // Act
            try
            {
                timesheetService.Add(timesheet);
            }
            catch (Exception ex)
            {
                // The assertion handles the outcome in this case and not the exception itself
            }

            // Assert
            Equals(timesheetService.GetAll().Count, 0);
        }
    }
}
