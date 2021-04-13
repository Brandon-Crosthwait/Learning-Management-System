using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using LMS.Data;
using LMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace LMS.Pages.Courses.CourseInfo.Assignments
{
    public class CreateModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;

        public CreateModel(LMS.Data.LMSContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Course Course { get; set; }

        [BindProperty]
        public Assignment Assignment { get; set; }

        public SelectList SubmissionList { get; set; }

        public IActionResult OnGet()
        {
            List<SelectListItem> types = new List<SelectListItem>();

            types.Add(new SelectListItem() { Text = "Text Box Entry", Value = "Text Box Entry" });
            types.Add(new SelectListItem() { Text = "File Upload", Value = "File Upload" });

            SubmissionList = new SelectList(types, "Value", "Text");

            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            int courseID = (int)HttpContext.Session.GetInt32("currCourse");

            await CreateAssignment(courseID);

            return RedirectToPage("./Index");
        }

        public async Task CreateAssignment(int courseID)
        {
            int userID = (int)HttpContext.Session.GetInt32("userID");

            Course = await _context.Course.FirstOrDefaultAsync(m => m.ID == courseID);

            Assignment.CourseID = Course.ID;

            _context.Assignment.Add(Assignment);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Creates a notification for when a new assignment is added to a course.
        /// </summary>
        /// <param name="userID">int userID</param>
        /// <returns>Task</returns>
        private async Task AddAssignmentCreatedNotification(int userID)
        {
            Course course = await _context.Course.FirstOrDefaultAsync(a => a.ID == Assignment.CourseID);
            Department department = await _context.Department.FirstOrDefaultAsync(d => d.ID == Int32.Parse(course.Department));
            Assignment assignment =  await _context.Assignment.FirstOrDefaultAsync(a => a.Description == Assignment.Description);

            string message = "New assignment available: " + department.Code + " " + course.Number + " - " + Assignment.Title;

            List<Registration> registrationRecords = new List<Registration>();
            registrationRecords = _context.Registration.Where(r => r.Course == course.ID).ToList();

            foreach (Registration registration in registrationRecords)
            {
                    Notification notification = new Notification()
                    {
                        StudentID = registration.Student,
                        AssignmentID = assignment.ID,
                        Message = message,
                    };
                    _context.Notification.Add(notification);
            }
            await _context.SaveChangesAsync();
        }
    }
}
