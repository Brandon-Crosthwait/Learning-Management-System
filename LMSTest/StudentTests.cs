using System.Linq;
using LMS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Extensions.DependencyInjection;
using LMS.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;

namespace LMSTest
{
    [TestClass]
    public class StudentTests
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        [TestMethod]
        public async Task SubmitAssignment()
        {
            // Arrange
            var optionsBuilder = new DbContextOptionsBuilder<LMSContext>().UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=LMS_BLUE;User ID=LMS_BLUE;Password=BTlms2021!");
            var _context = new LMSContext(optionsBuilder.Options);
            //LMS.Pages.Submission.CreateModel model = new LMS.Pages.Submission.CreateModel(_context);

            int userID = 1;
            int assignmentID = 14;

            var pageModel = new LMS.Pages.Submission.CreateModel(_context)
            {
                StudentID = userID,
                AssignmentID = assignmentID,
                Submission = new Submission(),
                Content = "This is a test submission."
            };

            /** Act **/
            List<Submission> submissionRecords;
            submissionRecords = _context.Submission.Where(s => s.StudentID == userID).ToList();
            int preCount = submissionRecords.Count();

            await pageModel.SubmitAssignment(userID, assignmentID);

            submissionRecords = _context.Submission.Local.Where(s => s.StudentID == userID).ToList();
            int postCount = submissionRecords.Count();

            /** Compare **/
            Assert.AreNotEqual(preCount, postCount);

            /** Cleanup **/
            var assignmentSubmission = _context.Submission.FirstOrDefault(s => s.StudentID == userID && s.AssignmentID == assignmentID);
            _context.Submission.Remove(assignmentSubmission);
            _context.SaveChanges();

            //    User user = new User();
            //    user = _context.User.FirstOrDefault(x => x.ID == 1);

            //    Course course = new Course();
            //    course = _context.Course.FirstOrDefault(x => x.ID == 7);

            //    Assignment assignment = new Assignment();
            //    assignment = _context.Assignment.First(x => x.ID == 14);

            //    Submission assignmentSubmission = new Submission()
            //    {
            //        //AssignmentID = assignment.ID,
            //        //StudentID = user.ID,
            //        Content = "This is a test submission",
            //        //Grade = "--"
            //    };

            //    model.Student = user;
            //    model.Course = course;
            //    model.Assignment = assignment;
            //    model.Submission = assignmentSubmission;

            //    // Act
            //    await model.SubmitAssignment(user.ID, assignment.ID);

            //    // Assert
            //    Assert.IsTrue(true);

            //}

            //public void ConfigureServices(IServiceCollection services)
            //{
            //    services.AddDistributedMemoryCache();
            //    services.AddHttpContextAccessor();
            //    services.AddSession(options =>
            //    {
            //        options.Cookie.Name = ".lms.session";
            //        options.IdleTimeout = TimeSpan.FromSeconds(600);
            //        options.Cookie.HttpOnly = true;
            //        options.Cookie.IsEssential = true;
            //    });
            //    services.AddDbContext<LMSContext>(options =>
            //            options.UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=LMS_BLUE;User ID=LMS_BLUE;Password=Blue2021!"));
        }
    }
}
