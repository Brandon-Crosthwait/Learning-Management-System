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
        public async Task SubmitAssignmentTest()
        {
            // Arrange
            var optionsBuilder = new DbContextOptionsBuilder<LMSContext>().UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=LMS_BLUE;User ID=LMS_BLUE;Password=BTlms2021!");
            var testContext = new LMSContext(optionsBuilder.Options);
            //LMS.Pages.Submission.CreateModel model = new LMS.Pages.Submission.CreateModel(_context);

            int userID = 1;
            int assignmentID = 14;

            var pageModel = new LMS.Pages.Submission.CreateModel(testContext, webHostEnvironment)
            {
                StudentID = userID,
                AssignmentID = assignmentID,
                Submission = new Submission(),
                Content = "This is a test submission."
            };

            /** Act **/
            List<Submission> submissionRecords;
            submissionRecords = testContext.Submission.Where(s => s.StudentID == userID).ToList();
            int preCount = submissionRecords.Count();

            await pageModel.SubmitAssignment(userID, assignmentID);

            submissionRecords = testContext.Submission.Local.Where(s => s.StudentID == userID).ToList();
            int postCount = submissionRecords.Count();

            /** Compare **/
            Assert.AreNotEqual(preCount, postCount);

            /** Cleanup **/
            var assignmentSubmission = testContext.Submission.FirstOrDefault(s => s.StudentID == userID && s.AssignmentID == assignmentID);
            testContext.Submission.Remove(assignmentSubmission);
            testContext.SaveChanges();
        }

        [TestMethod]
        public async Task PaymentTest()
            {
                /** Arrange **/
                var optionsBuilder = new DbContextOptionsBuilder<LMSContext>().UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=LMS_BLUE;User ID=LMS_BLUE;Password=BTlms2021!");
                LMS.Data.LMSContext context = new LMSContext(optionsBuilder.Options);
                int userID = 1;
                User User = context.User.Where(u => u.ID == userID).FirstOrDefault();

                var pageModel = new LMS.Pages.Tuition.IndexModel(context)
                {
                    UserID = userID,
                };

                /** Act **/
                int oldPayment = User.Payment;

                string cardnumber = "4242424242424242";
                string cvc = "595";
                string month = "08";
                string year = "23";
                int amount = 100;

                await pageModel.processPayment(cardnumber, cvc, month, year, amount, userID);

                int newPayment = User.Payment;

                /** Compare **/
                Assert.AreNotEqual(oldPayment, newPayment);
            }

        [TestMethod]
        public void StudentRegisterCourseTest()
        {
            /** Arrange **/
            var optionsBuilder = new DbContextOptionsBuilder<LMSContext>().UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=LMS_BLUE;User ID=LMS_BLUE;Password=BTlms2021!");
            LMS.Data.LMSContext context = new LMSContext(optionsBuilder.Options);
            int userID = 1010;

            var pageModel = new LMS.Pages.Registrations.IndexModel(context)
            {
                UserID = userID,
                Registration = new Registration(),
            };

            /** Act **/
            List<Registration> registrationRecords;
            registrationRecords = context.Registration.Where(r => r.Student == userID).ToList();
            int preCount = registrationRecords.Count();

            int courseID = 10;
            pageModel.AddCourse(courseID);

            registrationRecords = context.Registration.Local.Where(r => r.Student == userID).ToList();
            int postCount = registrationRecords.Count();

            /** Compare **/
            Assert.AreNotEqual(preCount, postCount);
        }

        [TestMethod]
        public void ToDoListOrderAssignmentsTest()
        {
            /** Arrange **/
            var optionsBuilder = new DbContextOptionsBuilder<LMSContext>().UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=LMS_BLUE;User ID=LMS_BLUE;Password=BTlms2021!");
            LMS.Data.LMSContext context = new LMSContext(optionsBuilder.Options);

            Assignment Assignment1 = new Assignment()
            {
                Title = "TestAssignment1",
                Points = 100,
                Description = "One year ago",
                Due = DateTime.Now.AddDays(-365),
                SubmissionType = "Test Box Entry",
                CourseID = 8,
            };

            Assignment Assignment2 = new Assignment()
            {
                Title = "TestAssignment2",
                Points = 100,
                Description = "Six months ago",
                Due = DateTime.Now.AddDays(-182),
                SubmissionType = "Test Box Entry",
                CourseID = 8,
            };

            Assignment Assignment3 = new Assignment()
            {
                Title = "TestAssignment3",
                Points = 100,
                Description = "One month Ahead",
                Due = DateTime.Now.AddMonths(1),
                SubmissionType = "Test Box Entry",
                CourseID = 8,
            };

            Assignment Assignment4 = new Assignment()
            {
                Title = "TestAssignment4",
                Points = 100,
                Description = "Two months ahead",
                Due = DateTime.Now.AddMonths(2),
                SubmissionType = "Test Box Entry",
                CourseID = 8,
            };

            Assignment Assignment5 = new Assignment()
            {
                Title = "TestAssignment5",
                Points = 100,
                Description = "Three months ahead",
                Due = DateTime.Now.AddMonths(3),
                SubmissionType = "Test Box Entry",
                CourseID = 8,
            };

            Assignment Assignment6 = new Assignment()
            {
                Title = "TestAssignment6",
                Points = 100,
                Description = "Four months ahead",
                Due = DateTime.Now.AddMonths(4),
                SubmissionType = "Test Box Entry",
                CourseID = 8,
            };

            Assignment Assignment7 = new Assignment()
            {
                Title = "TestAssignment7",
                Points = 100,
                Description = "Five months ahead",
                Due = DateTime.Now.AddMonths(5),
                SubmissionType = "Test Box Entry",
                CourseID = 8,
            };


            Assignment Assignment8 = new Assignment()
            {
                Title = "TestAssignment8",
                Points = 100,
                Description = "Six months ahead",
                Due = DateTime.Now.AddMonths(6),
                SubmissionType = "Test Box Entry",
                CourseID = 8,
            };


            List<Assignment> TestAssignmentList = new List<Assignment>();
            TestAssignmentList.Add(Assignment1);
            TestAssignmentList.Add(Assignment2);
            TestAssignmentList.Add(Assignment3);
            TestAssignmentList.Add(Assignment4);
            TestAssignmentList.Add(Assignment5);
            TestAssignmentList.Add(Assignment6);
            TestAssignmentList.Add(Assignment7);
            TestAssignmentList.Add(Assignment8);

            var pageModel = new LMS.Pages.UserHomeModel(context)
            {
                AssignmentList = TestAssignmentList,
            };

            /** Act **/
            pageModel.OrderAssignments();
            TestAssignmentList = pageModel.AssignmentList;

            /** Assert **/
            Assert.IsTrue(TestAssignmentList.Count == 5);

            int temp = 0;
            for (int i = 1; i < TestAssignmentList.Count; i++)
            {
                Assert.IsTrue(TestAssignmentList[temp].Due <= TestAssignmentList[i].Due);
                temp = i;
            }
        }

        [TestMethod]
        public async Task LoginTest()
            {
                /** Arrange **/
                var optionsBuilder = new DbContextOptionsBuilder<LMSContext>().UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=LMS_BLUE;User ID=LMS_BLUE;Password=BTlms2021!");
                LMS.Data.LMSContext context = new LMSContext(optionsBuilder.Options);
                int userID = 1;
                User User = context.User.Where(u => u.ID == userID).FirstOrDefault();

                var pageModel = new LMS.Pages.Tuition.IndexModel(context)
                {
                    UserID = userID,
                };

                /** Act **/
                string Password = "11111111";
                string cvc = "595";
                string month = "08";
                string year = "23";
                int amount = 100;

                await pageModel.processPayment(cardnumber, cvc, month, year, amount, userID);

                int newPayment = User.Payment;

                /** Compare **/
                Assert.AreNotEqual(oldPayment, newPayment);
            }
    }
}
