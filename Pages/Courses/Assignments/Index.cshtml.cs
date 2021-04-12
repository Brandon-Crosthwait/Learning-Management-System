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

        public List<double> Totals { get; set; }

        public List<double> Grades { get; set; }

        [BindProperty]
        public Course Course { get; set; }

        [BindProperty]
        public Department Department { get; set; }

        public bool BoolInstructor { get; set; }

        public int StudentID { get; set; }

        public double PointsPossible { get; set; }

        public double CourseGrade { get; set; }

        public async Task OnGetAsync()
        {
            bool sessionInstructor = bool.Parse(HttpContext.Session.GetString("isInstructorSession"));
            StudentID = (int)HttpContext.Session.GetInt32("userID");

            // Retrieve the selected Course to display proper course info
            int courseID = (int)HttpContext.Session.GetInt32("currCourse");
            Course = await _context.Course.FirstOrDefaultAsync(m => m.ID == courseID);

            Submissions = await _context.Submission.Where(x => x.StudentID == StudentID).ToListAsync();

            // Retrieve the Department so that the code can be displayed on page
            if (Int32.TryParse(Course.Department, out int departmentID))
            {
                Department = await _context.Department.FirstOrDefaultAsync(m => m.ID == departmentID);
            }

            // Display the list of Assignments for the selected Course
            Assignment = await _context.Assignment.Where(x => x.CourseID == courseID).ToListAsync();

            //Totals = new List<double>();
            //Grades = new List<double>();

            //foreach (var item in Assignment)
            //{
            //    foreach (var submission in Submissions)
            //    {
            //        if (item.ID == submission.AssignmentID)
            //        {
            //            if (submission.Grade != "--")
            //            {
            //                double total = item.Points;
            //                Totals.Add(total);
            //            }
            //        }
            //    }
            //}

            //foreach (var item in Totals)
            //{
            //    PointsPossible += item;
            //}

            //foreach (var item in Assignment)
            //{
            //    foreach (var submission in Submissions)
            //    {
            //        if (item.ID == submission.AssignmentID)
            //        {
            //            if (submission.Grade != "--")
            //            {
            //                double grade = Double.Parse(submission.Grade);
            //                Grades.Add(grade);
            //            }
            //        }
            //    }
            //}

            //foreach (var item in Grades)
            //{
            //    CourseGrade += item;
            //}

            //CourseGrade = (CourseGrade / PointsPossible) * 100;

            if (sessionInstructor == true)
            {
                BoolInstructor = true;
            } 
            else
            {
                BoolInstructor = false;
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
