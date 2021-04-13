using System.Linq;
using LMS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LMS.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace LMSTest
{
    [TestClass]
    public class InstructorTests
    {
        [TestMethod]
        public async Task CourseCreateTest()
        {
            // Arrange
            var optionsBuilder = new DbContextOptionsBuilder<LMSContext>().UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=LMS_BLUE;User ID=LMS_BLUE;Password=BTlms2021!");
            var _context = new LMSContext(optionsBuilder.Options);

            LMS.Pages.Courses.CreateModel model = new LMS.Pages.Courses.CreateModel(_context);

            User user = new User()
            {
                FirstName = "David",
                LastName = "Smith",
                ID = 4,
                IsInstructor = true,
            };

            Course insertCourse = new Course()
            {
                Number = 3030,
                Name = "InstructorTest",
                Department = "CS",
                Description = "Instructor test course",
                Location = "Test Location",
                CreditHours = 4,
                Capacity = 0
            };

            model.Course = insertCourse;
            model.Monday = true;
            model.Wednesday = true;
            model.Friday = true;
            model.StartTime = new DateTime(2021, 3, 22, 6, 30, 0);
            model.EndTime = new DateTime(2021, 3, 22, 8, 50, 0);

            // Act
            int precount = _context.Course.Where(c => c.InstructorID == user.ID).ToList().Count();
            await model.CreateCourse(user);
            int postcount = _context.Course.Where(c => c.InstructorID == user.ID).ToList().Count();

            // Assert
            Assert.AreNotEqual(precount, postcount);

            // Cleanup
            _context.Course.Remove(insertCourse);
            _context.SaveChanges();
        }

        [TestMethod]
        public async Task AssignmentCreateTest()
        {
            /** Arrange **/
            var optionsBuilder = new DbContextOptionsBuilder<LMSContext>().UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=LMS_BLUE;User ID=LMS_BLUE;Password=BTlms2021!");
            LMS.Data.LMSContext context = new LMSContext(optionsBuilder.Options);
            int userID = 1007;
            int courseID = 6;

            Assignment TestAssignment = new Assignment();
            TestAssignment.Title = "UnitTestAssignment1001";
            TestAssignment.Points = 30;
            TestAssignment.Description = "Unit Tests / InstructorTests / AssignmentCreateTest";
            TestAssignment.Due = DateTime.Parse("2021-04-28 23:59:00");
            TestAssignment.SubmissionType = "File Upload";

            var pageModel = new LMS.Pages.Courses.CourseInfo.Assignments.CreateModel(context)
            {
                Assignment = TestAssignment
            };

            /** Act **/
            List<Assignment> assignmentRecords;
            assignmentRecords = context.Assignment.Where(a => a.CourseID == courseID).ToList();
            int preCount = assignmentRecords.Count();

            await pageModel.CreateAssignment(courseID);

            assignmentRecords = context.Assignment.Local.Where(a => a.CourseID == courseID).ToList();
            int postCount = assignmentRecords.Count();

            /** Compare **/
            Assert.AreNotEqual(preCount, postCount);

            /** Cleanup **/
            var newAssignment = context.Assignment.FirstOrDefault(a => a.CourseID == courseID && a.Title == TestAssignment.Title);
            context.Assignment.Remove(newAssignment);
            context.SaveChanges();
        }

        [TestMethod]
        public async Task GradeAssignmentTest()
        {
            /** Arrange **/
            var optionsBuilder = new DbContextOptionsBuilder<LMSContext>().UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=LMS_BLUE;User ID=LMS_BLUE;Password=BTlms2021!");
            LMS.Data.LMSContext context = new LMSContext(optionsBuilder.Options);

            LMS.Pages.Submission.EditModel model = new LMS.Pages.Submission.EditModel(context);
            int submissionID = 138; 
            string grade = "10";
            var submission = await context.Submission.FirstOrDefaultAsync(x => x.ID == submissionID);

            /** Act **/
            await model.SubmitGrade(grade, submission);
            submission = await context.Submission.FirstOrDefaultAsync(x => x.ID == submissionID);
            var postValue = submission.Grade;

            /** Compare **/
            Assert.AreEqual("10", postValue);

            /** Cleanup **/
            await model.SubmitGrade("--", submission);



        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            services.AddSession(options =>
            {
                options.Cookie.Name = ".lms.session";
                options.IdleTimeout = TimeSpan.FromSeconds(600);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddDbContext<LMSContext>(options =>
                    options.UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=LMS_BLUE;User ID=LMS_BLUE;Password=Blue2021!"));
        }
    }
}


