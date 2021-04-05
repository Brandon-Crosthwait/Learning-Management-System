using System.Linq;
using LMS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LMS.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace LMSTest
{
    [TestClass]
    public class InstructorTests
    {
        [TestClass]
        public class UnitTest1
        {
            [TestMethod]
            public void StudentRegisterCourse()
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

        }
    }
}

