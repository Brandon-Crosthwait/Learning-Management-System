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


        public int one;
        public int two;
        public int three;
        public int four;
        public int five;
        public int six;
        public int seven;
        public double average;
        public int scount;
        public double placeholder;

        public string ann1;
        public string ann2;
        public string ann3;
        public string ann4;
        public string ann5;
        public string ann6;
        public string ann7;

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

                    Assignment = _context.Assignment.Where(u => u.ID == Submission.AssignmentID).FirstOrDefault();

                    if(Submission.Grade != "--"){
                        placeholder = ((double.Parse(Submission.Grade)/Assignment.Points)*100);
                        if(placeholder >= 95){
                            ann7 = "▇";
                        }
                        else if(placeholder >= 90){
                            ann6 = "▇";
                        }
                        else if(placeholder >= 85){
                            ann5 = "▇";
                        }
                        else if(placeholder >= 80){
                            ann4 = "▇";
                        }
                        else if(placeholder >= 75){
                            ann3 = "▇";
                        }
                        else if(placeholder >= 70){
                            ann2 = "▇";
                        }
                        else if(placeholder < 70){
                            ann1 = "▇";
                        }
                    }
                    break;
                }
                else
                {
                    Submitted = false;
                }
            }

            one = 0;
            two = 0;
            three = 0;
            four = 0;
            five = 0;
            six = 0;
            seven = 0;
            average = 0;
            scount = 0;

            foreach (var item in SubmissionsByAssignment)
            {
                Assignment = _context.Assignment.Where(u => u.ID == item.AssignmentID).FirstOrDefault();
                
                if(item.Grade != "--"){
                    placeholder = ((double.Parse(item.Grade)/Assignment.Points)*100);
                    average = average + placeholder;
                    scount = scount + 1;

                    if(placeholder >= 95){
                        seven = seven + 1;
                    }
                    else if(placeholder >= 90){
                        six = six + 1;
                    }
                    else if(placeholder >= 85){
                        five = five + 1;
                    }
                    else if(placeholder >= 80){
                        four = four + 1;
                    }
                    else if(placeholder >= 75){
                        three = three + 1;
                    }
                    else if(placeholder >= 70){
                        two = two + 1;
                    }
                    else if(placeholder < 70){
                        one = one + 1;
                    }
                }

            }
            average = Math.Round((average / scount), 2);

            return Page();
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
