using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace LMS.Pages.Submission
{
    public class CreateModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public CreateModel(LMS.Data.LMSContext context,
                            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        public User Student { get; set; }

        public Assignment Assignment { get; set; }

        public Course Course { get; set; }

        public Department Department { get; set; }

        public int[] GradesByAssignment { get; set; }

        [BindProperty]
        public LMS.Models.Submission Submission { get; set; }

        [BindProperty]
        public int AssignmentID { get; set; }

        [BindProperty]
        public int StudentID { get; set; }

        [BindProperty]
        public bool Submitted { get; set; }

        [BindProperty]
        public bool FileUpload { get; set; }

        [BindProperty]
        public string Content { get; set; }

        [BindProperty]
        public IFormFile File { get; set; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        public IActionResult OnGet()
        {
            StudentID = (int)HttpContext.Session.GetInt32("userID");
            AssignmentID = (int)HttpContext.Session.GetInt32("currAssignment");

            Student = _context.User.Where(u => u.ID == StudentID).FirstOrDefault();
            Assignment = _context.Assignment.Where(u => u.ID == AssignmentID).FirstOrDefault();
            Course = _context.Course.Where(u => u.ID == Assignment.CourseID).FirstOrDefault();
            Department = _context.Department.Where(u => u.ID == Int32.Parse(Course.Department)).FirstOrDefault();

            List<LMS.Models.Submission> SubmissionsByAssignment = new List<LMS.Models.Submission>();
            SubmissionsByAssignment = _context.Submission.Where(u => u.AssignmentID == AssignmentID).ToList();

            this.PopulateGraphData(SubmissionsByAssignment);

            Submitted = false;

            // Check for submission type to display proper form
            if (Assignment.SubmissionType == "File Upload")
            {
                FileUpload = true;
            }
            else
            {
                FileUpload = false;
            }

            //Finds assignments for instructor courses
            foreach (LMS.Models.Submission submission in SubmissionsByAssignment)
            {
                if (submission.StudentID == StudentID)
                {
                    Submitted = true;
                    Submission = submission;
                    break;
                }
                else
                {
                    Submitted = false;
                }
            }

            return Page();
        }

        private void PopulateGraphData(List<Models.Submission> submissionsByAssignment)
        {
            var gradedSubmissions = submissionsByAssignment.Where(g => g.Grade != "--").ToList();
            var points = Assignment.Points;
            int[] data = new int[points];
            for (int i = 0; i <= points; i++)
            {
                data[i] = gradedSubmissions.Count(g => int.Parse(g.Grade) == i);
            }
            this.GradesByAssignment = data;
        }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            StudentID = (int)HttpContext.Session.GetInt32("userID");
            AssignmentID = (int)HttpContext.Session.GetInt32("currAssignment");

            Student = _context.User.Where(u => u.ID == StudentID).FirstOrDefault();
            Assignment = _context.Assignment.Where(u => u.ID == AssignmentID).FirstOrDefault();
            Course = _context.Course.Where(u => u.ID == Assignment.CourseID).FirstOrDefault();
            Department = _context.Department.Where(u => u.ID == Int32.Parse(Course.Department)).FirstOrDefault();

            // Assign the student and assignment to the submission record
            Submission.StudentID = StudentID;
            Submission.AssignmentID = AssignmentID;
            Submission.Grade = "--";

            if (Assignment.SubmissionType == "File Upload")
            {
                // Assign the file to the submission record
                if (File != null)
                {
                    if (Submission.Content != null)
                    {
                        string filePath = Path.Combine(webHostEnvironment.WebRootPath,
                        "StudentSubmissions", Submission.Content);
                        System.IO.File.Delete(filePath);
                    }
                    Submission.Content = ProcessUploadedFile(Student, Course, Department);

                    _context.Submission.Add(Submission);
                    await _context.SaveChangesAsync();

                    HttpContext.Session.SetInt32("currCourse", Assignment.CourseID);
                    return RedirectToPage("/Courses/Assignments/Index");
                }
                else
                {
                    return Page();
                }
            }
            else
            {
                Submission.Content = Content;
                _context.Submission.Add(Submission);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetInt32("currCourse", Assignment.CourseID);
                return RedirectToPage("/Courses/Assignments/Index");
            }
        }

        private string ProcessUploadedFile(User Student, Course Course, Department Department)
        {
            string uniqueFileName = null;
            string filePath = null;

            if (File != null)
            {
                string studentName = Student.FirstName.ToLower() + Student.LastName.ToLower();
                studentName = studentName.Replace(" ", String.Empty);
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "StudentSubmissions/" + Department.Code + Course.Number + "/" + Student.ID + "_" + studentName);
                if (!System.IO.Directory.Exists(uploadsFolder))
                {
                    System.IO.Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = File.FileName;
                filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    File.CopyTo(fileStream);
                }
            }

            return filePath;
        }
    }
}
