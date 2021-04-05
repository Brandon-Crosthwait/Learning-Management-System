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

