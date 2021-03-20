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

        [BindProperty]
        public int AssignmentID { get; set; }
        [BindProperty]
        public int StudentID { get; set; }
        [BindProperty]
        public IFormFile File { get; set; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        public IActionResult OnGet()
        {
            StudentID = (int)HttpContext.Session.GetInt32("userID");
            AssignmentID = (int)HttpContext.Session.GetInt32("currAssignment");
            return Page();
        }

        [BindProperty]
        public LMS.Models.Submission Submission { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            StudentID = (int)HttpContext.Session.GetInt32("userID");
            AssignmentID = (int)HttpContext.Session.GetInt32("currAssignment");

            Student = _context.User.Where(u => u.ID == StudentID).FirstOrDefault();
            Assignment = _context.Assignment.Where(u => u.ID == AssignmentID).FirstOrDefault();
            Course = _context.Course.Where(u => u.ID == Assignment.CourseID).FirstOrDefault();
            Department = _context.Department.Where(u => u.ID == Int32.Parse(Course.Department)).FirstOrDefault();

            if (File != null)
            {
                if (Submission.Content != null)
                {
                    string filePath = Path.Combine(webHostEnvironment.WebRootPath,
                    "StudentSubmissions", Submission.Content);
                    System.IO.File.Delete(filePath);
                }
                Submission.Content = ProcessUploadedFile(Student, Course, Department);
            }

            _context.Submission.Add(Submission);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
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
