using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Http;

namespace LMS.Pages.Registrations
{
    public class IndexModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;

        public IndexModel(LMS.Data.LMSContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public SelectList Departments { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Department { get; set; }

        public string DepartmentID { get; set; }

        public IList<Course> Course { get; set; }

        public IList<Registration> RegList { get; set; }
        public string registerButton = "Register";
        public string buttonStyle = "btn-primary";

        [BindProperty]
        public int UserID { get; set; }
        public User User { get; set; }

        [BindProperty]
        public Registration Registration { get; set; }

        public async Task OnGetAsync()
        {
            UserID = (int)HttpContext.Session.GetInt32("userID");
            var Courses = from c in _context.Course
                          select c;

            IQueryable<string> departmentNameQuery = from d in _context.Department
                              orderby d.Name
                              select d.Name;

            RegList = _context.Registration.Where(u => u.Student == UserID).ToList();
            if (!string.IsNullOrEmpty(SearchString))
            {
                Courses = Courses.Where(s => s.Name.Contains(SearchString));
            }

            if (!string.IsNullOrEmpty(Department))
            {
                IQueryable<int> departmentIdQuery = from d in _context.Department.Where(x => x.Name == Department)
                                                    select d.ID;
                DepartmentID = departmentIdQuery.First().ToString();
                Courses = Courses.Where(x => x.Department == DepartmentID);
            }

            Departments = new SelectList(await departmentNameQuery.Distinct().ToListAsync());
            Course = await Courses.ToListAsync();
            
        }

        public async Task<IActionResult> OnPostAsync(int course)
        {
            UserID = (int)HttpContext.Session.GetInt32("userID");

            RegList = _context.Registration.Where(u => u.Student == UserID).ToList();
            var currRegistration = RegList.Where(u => u.Course == course).FirstOrDefault();
            if (currRegistration != null)
            {
                _context.Registration.Remove(currRegistration);
            }
            else
            {
                AddCourse(course);

                await AddAssignmentNotifications(course);
            }
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        private async Task AddAssignmentNotifications(int courseID)
        {
            Course course = await _context.Course.FirstOrDefaultAsync(c => c.ID == courseID);
            Department department = await _context.Department.FirstOrDefaultAsync(d => d.ID == Int32.Parse(course.Department));
            List<Assignment> assignmentRecords = new List<Assignment>();
            assignmentRecords = _context.Assignment.Where(a => a.CourseID == course.ID).ToList();

            foreach (Assignment assignment in assignmentRecords)
            {
                //Filters out old assignments
                if (assignment.Due >= DateTime.Now)
                {
                    string message = "New assignment available: " + department.Code + " " + course.Number + " - " + assignment.Title;

                    Notification notification = new Notification()
                    {
                        StudentID = UserID,
                        AssignmentID = assignment.ID,
                        Message = message,
                    };
                    _context.Notification.Add(notification);
                }
            }
        }

        public void AddCourse(int course)
        {
            Registration.Course = course;
            Registration.Student = UserID;
            Registration.Grade = 0;
            _context.Registration.Add(Registration);
        }
    }
}