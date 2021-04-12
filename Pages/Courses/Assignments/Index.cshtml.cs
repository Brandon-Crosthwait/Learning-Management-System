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

namespace LMS.Pages.Courses.CourseInfo.Assignments
{
    public class IndexModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;

        public IndexModel(LMS.Data.LMSContext context)
        {
            _context = context;
        }

        public IList<Assignment> Assignment { get;set; }

        public IList<LMS.Models.Submission> Submissions { get; set; }

        public Registration Registration { get; set; }

        [BindProperty]
        public Course Course { get; set; }

        [BindProperty]
        public Department Department { get; set; }

        public bool BoolInstructor { get; set; }

        public int StudentID { get; set; }

        public double PointsPossible { get; set; }

        public double CourseGrade { get; set; }

        public IList<Registration> Registration {get; set;}
        public Registration StudentRegistration { get; set; }

        public int one;
        public int two;
        public int three;
        public int four;
        public int five;
        public int six;
        public int seven;
        public double average;
        public int scount;

        public string ann1;
        public string ann2;
        public string ann3;
        public string ann4;
        public string ann5;
        public string ann6;
        public string ann7;

        public async Task OnGetAsync()
        {
            bool sessionInstructor = bool.Parse(HttpContext.Session.GetString("isInstructorSession"));
            StudentID = (int)HttpContext.Session.GetInt32("userID");

            // Retrieve the selected Course to display proper course info
            int courseID = (int)HttpContext.Session.GetInt32("currCourse");
            Course = await _context.Course.FirstOrDefaultAsync(m => m.ID == courseID);
            
            Submissions = await _context.Submission.Where(x => x.StudentID == StudentID).ToListAsync();

            Registration = await _context.Registration.Where(c => c.Course == courseID).ToListAsync();
            StudentRegistration = await _context.Registration.Where(u => u.Student == StudentID).FirstOrDefaultAsync();
            
            // Retrieve the Department so that the code can be displayed on page
            if (Int32.TryParse(Course.Department, out int departmentID))
            {
                Department = await _context.Department.FirstOrDefaultAsync(m => m.ID == departmentID);
            }

            // Display the list of Assignments for the selected Course
            Assignment = await _context.Assignment.Where(x => x.CourseID == courseID).ToListAsync();
            
            one = 0;
            two = 0;
            three = 0;
            four = 0;
            five = 0;
            six = 0;
            seven = 0;
            average = 0;
            scount = 0;

            foreach (var item in Registration)
            {   
                    average = average + item.Grade;
                    scount = scount + 1;

                    if(item.Grade >= 95){
                        seven = seven + 1;
                    }
                    else if(item.Grade >= 90){
                        six = six + 1;
                    }
                    else if(item.Grade >= 85){
                        five = five + 1;
                    }
                    else if(item.Grade >= 80){
                        four = four + 1;
                    }
                    else if(item.Grade >= 75){
                        three = three + 1;
                    }
                    else if(item.Grade >= 70){
                        two = two + 1;
                    }
                    else if(item.Grade < 70){
                        one = one + 1;
                    }
            }
            
            average = Math.Round((average / scount), 2);

            // Get Registration to display Grade
            Registration = await _context.Registration.FirstOrDefaultAsync(r => r.Student == StudentID && r.Course == courseID);

            if (sessionInstructor == true)
            {
                BoolInstructor = true;
            } 
            else
            {
                BoolInstructor = false;
                        
                        if(StudentRegistration.Grade >= 95){
                            ann7 = "▇";
                        }
                        else if(StudentRegistration.Grade >= 90){
                            ann6 = "▇";
                        }
                        else if(StudentRegistration.Grade >= 85){
                            ann5 = "▇";
                        }
                        else if(StudentRegistration.Grade >= 80){
                            ann4 = "▇";
                        }
                        else if(StudentRegistration.Grade >= 75){
                            ann3 = "▇";
                        }
                        else if(StudentRegistration.Grade >= 70){
                            ann2 = "▇";
                        }
                        else if(StudentRegistration.Grade < 70){
                            ann1 = "▇";
                        }
            }
        }

        public async Task<IActionResult> OnPostAsync(int assignment)
        {
            string IsInstructor = HttpContext.Session.GetString("isInstructorSession");

            if (assignment != 0)
            {
                HttpContext.Session.SetInt32("currAssignment", assignment);

                return new RedirectToPageResult("/Submission/Create");
            }
            return new RedirectToPageResult("/Assignments");
        }
    }
}
