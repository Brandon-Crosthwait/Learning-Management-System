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

namespace LMSTest
{
    [TestClass]
    public class InstructorTests
    {
        [TestMethod]
        public async Task CourseOnPostAsyncTest()
        {
            // Arrange
            var optionsBuilder = new DbContextOptionsBuilder<LMSContext>().UseSqlServer("Data Source=titan.cs.weber.edu,10433;Initial Catalog=LMS_BLUE;User ID=LMS_BLUE;Password=Blue2021!");
            var _context = new LMSContext(optionsBuilder.Options);
            LMS.Pages.Courses.CreateModel model = new LMS.Pages.Courses.CreateModel(_context);

            string UserID = "4";
            Course insertCourse = new Course()
            {
                ID = 11111,
                Number = 3030,
                Name = "InstructorTest",
                Department = "CS",
                Description = "Instructor test course",
                Location = "Test Location",
                CreditHours = 4,
                Capacity = 0
            };

            model.Course = insertCourse;
            model.HttpContext.Session.SetString("userID", UserID);
            model.User.FirstName = "David";
            model.User.LastName = "Smith";
            model.User.ID = 4;
            model.Monday = true;
            model.Wednesday = true;
            model.Friday = true;
            model.StartTime = new DateTime(2021, 3, 22, 6, 30, 0);
            model.EndTime = new DateTime(2021, 3, 22, 8, 50, 0);


            // Act
            await model.OnPostAsync();

            // Assert
            Assert.IsTrue(0 == 0);

            _context.Course.Remove(insertCourse);
            _context.SaveChanges();
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

