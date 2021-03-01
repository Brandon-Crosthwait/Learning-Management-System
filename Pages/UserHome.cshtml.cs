using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using LMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Pages
{
    public class UserHomeModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;

        public UserHomeModel(LMS.Data.LMSContext context)
        {
            _context = context;
        }

        public string Username;
        public int UserID;

        public User User { get; set; }

        public IList<Course> CourseList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext != null)
            {
                // Grab the ID of the user who logged in
                UserID = (int)HttpContext.Session.GetInt32("userID");
                if (UserID <= 0)
                {
                    return new RedirectToPageResult("/Login");
                }
                else
                {
                    User = _context.User.Where(u => u.ID == UserID).FirstOrDefault();

                    //Stores boolean string for whether the user is an instructor or student
                    string IsInstructor = HttpContext.Session.GetString("isInstructorSession");

                    //Pulls data for cards
                    if (IsInstructor == "True")  //Instructor is logged in
                    {
                        var courses = from c in _context.Course
                                      join d in _context.Department on c.Department equals d.ID.ToString()
                                      where c.InstructorID == UserID

                                      select new Course
                                      {
                                          Number = c.Number,
                                          Name = c.Name,
                                          Instructor = c.Instructor,
                                          Location = c.Location,
                                          Days = c.Days,
                                          Time = c.Time,
                                          Department = d.Code
                                      };

                        CourseList = await courses.ToListAsync();

                    } else  //Student is logged in
                    {
                        var courses = from c in _context.Course
                                      join d in _context.Department on c.Department equals d.ID.ToString()
                                      join r in _context.Registration on c.ID equals r.CourseID
                                      join u in _context.User on r.UserID equals u.ID

                                      select new Course
                                      {
                                          Number = c.Number,
                                          Name = c.Name,
                                          Instructor = c.Instructor,
                                          Location = c.Location,
                                          Days = c.Days,
                                          Time = c.Time,
                                          Department = d.Code
                                      };

                         CourseList = await courses.ToListAsync();
                    }

                    return Page();
                }
            }
            else
            {
                return new RedirectToPageResult("/Login");
            }
        }
    }
}
