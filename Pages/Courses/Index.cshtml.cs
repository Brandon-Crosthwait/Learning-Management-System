using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Http;

namespace LMS.Pages.Courses
{
    public class IndexModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;

        public IndexModel(LMS.Data.LMSContext context)
        {
            _context = context;
        }

        public List<Course> Courses { get;set; }

        public int UserID { get; set; }
        public User User { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext != null)
            {
                UserID = (int)HttpContext.Session.GetInt32("userID");
                if (UserID <= 0)
                {
                    return new RedirectToPageResult("/Login");
                }
                else
                {
                    UserID = (int)HttpContext.Session.GetInt32("userID");

                    //Stores boolean string for whether the user is an instructor or student
                    string IsInstructor = HttpContext.Session.GetString("isInstructorSession");

                    
                    if (IsInstructor == "True")  //User is an instructor
                    {
                        User = _context.User.Where(u => u.ID == UserID).FirstOrDefault();
                        string instructor = User.LastName + ", " + User.FirstName;
                        Courses = _context.Course.Where(u => u.Instructor == instructor).ToList();
                    }
                    else  //User is a student
                    {
                        List<int> courseNum = new List<int>();

                        List<Registration> RegistrationRecords;

                        //Pulls registration records from db
                        RegistrationRecords = _context.Registration.Where(u => u.Student == UserID).ToList();

                        //Pulls course numbers for user
                        foreach (LMS.Models.Registration record in RegistrationRecords)
                        {
                            if (record.Student == UserID)
                            {
                                courseNum.Add(record.Course);
                            }
                        }

                        Courses = new List<Course>();

                        //Searches Course table for the user's course numbers
                        foreach (int num in courseNum)
                        {
                            Course course = _context.Course.Where(u => u.ID == num).FirstOrDefault();

                            if (course != null)
                            {
                                Courses.Add(course);
                            }
                        }
                    }
                    return Page();
                }
            }
            else
            {
                return new RedirectToPageResult("/Login");
            }
        }

        //public async Task OnGetAsync()
        //{
        //    UserID = (int)HttpContext.Session.GetInt32("userID");
        //    User = _context.User.Where(u => u.ID == UserID).FirstOrDefault();
        //    string instructor = User.LastName + ", " + User.FirstName;
        //    Course = await _context.Course.Where(u => u.Instructor == instructor).ToListAsync();
        //}
    }
}
