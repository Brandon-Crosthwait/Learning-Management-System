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
    }
}
