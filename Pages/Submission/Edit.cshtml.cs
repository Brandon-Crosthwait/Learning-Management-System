using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Http;

namespace LMS.Pages.Submission
{
    public class EditModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;

        public EditModel(LMS.Data.LMSContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LMS.Models.Submission Submission { get; set; }

        public User Student { get; set; }

        public Assignment Assignment { get; set; }

        public Course Course { get; set; }

        public Department Department { get; set; }

        [BindProperty]
        public string Grade { get; set; }

        [BindProperty]
        public bool FileUpload { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Submission = await _context.Submission.FirstOrDefaultAsync(x => x.ID == id);

            Student = await _context.User.FirstOrDefaultAsync(u => u.ID == Submission.StudentID);
            Assignment = await _context.Assignment.FirstOrDefaultAsync(u => u.ID == Submission.AssignmentID);
            Course = await _context.Course.FirstOrDefaultAsync(u => u.ID == Assignment.CourseID);
            Department = await _context.Department.FirstOrDefaultAsync(u => u.ID == Int32.Parse(Course.Department));

            if (Submission == null)
            {
                return NotFound();
            }

            // Check for submission type to display proper form
            if (Assignment.SubmissionType == "File Upload")
            {
                FileUpload = true;
            }
            else
            {
                FileUpload = false;
            }

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            Submission = await _context.Submission.FirstOrDefaultAsync(x => x.ID == id);

            HttpContext.Session.SetInt32("currAssignment", Submission.AssignmentID);

            Submission.Grade = Grade;

            //Stores notification in db
            await AddGradedNotification();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubmissionExists(Submission.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        /// <summary>
        /// Adds a notification when an instructor grades an assignment
        /// </summary>
        /// <returns>Task</returns>
        private async Task AddGradedNotification()
        {
            Assignment assignment = await _context.Assignment.FirstOrDefaultAsync(a => a.ID == Submission.AssignmentID);
            Course course = await _context.Course.FirstOrDefaultAsync(c => c.ID == assignment.CourseID);
            Department department = await _context.Department.FirstOrDefaultAsync(d => d.ID == Int32.Parse(course.Department));

            string message = department.Code + " " + course.Number + " - " + assignment.Title + " has been graded.";

            Notification notification = new Notification()
            {
                StudentID = Submission.StudentID,
                AssignmentID = Submission.AssignmentID,
                Message = message,
            };

            _context.Notification.Add(notification);
        }

        private bool SubmissionExists(int id)
        {
            return _context.Submission.Any(e => e.ID == id);
        }

        public ActionResult OnPostDownloadFile(int? submission)
        {
            Submission = _context.Submission.FirstOrDefault(x => x.ID == submission);
            Assignment = _context.Assignment.FirstOrDefault(x => x.ID == Submission.AssignmentID);
            string newFile;

            int idx = Submission.Content.LastIndexOf('\\');
            if (idx != -1)
            {
                newFile = Submission.StudentID + "_" + Submission.Content.Substring(idx + 1);

                idx = Submission.Content.LastIndexOf("root\\");
                string filePath = Submission.Content.Substring(idx + 5);

                return File(filePath, "application/octet-stream", newFile);
            }
            else
            {
                return Page();
            }
        }
    }
}
