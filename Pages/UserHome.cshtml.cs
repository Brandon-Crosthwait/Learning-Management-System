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
using System.Collections;

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

        public List<Assignment> AssignmentList { get; set; }
        public List<Assignment> Assignments { get; set; }

        public List<string> CourseInfo { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext != null)
            {
                // Grab the ID of the user who logged in
                UserID = (int)HttpContext.Session.GetInt32("userID");
                User = _context.User.Where(u => u.ID == UserID).FirstOrDefault();

                if (UserID <= 0)
                {
                    return new RedirectToPageResult("/Login");
                }
                else
                {
                    //Stores boolean string for whether the user is an instructor or student
                    string IsInstructor = HttpContext.Session.GetString("isInstructorSession");

                    /*** Populate instructor cards ***/

                    //Pulls data for cards
                    if (IsInstructor == "True")  //User is an instructor
                    {
                        //LinQ statement to filter data in db
                        var courses = from c in _context.Course
                                      join d in _context.Department on c.Department equals d.ID.ToString()
                                      where c.InstructorID == UserID

                                      select new Course
                                      {
                                          ID = c.ID,
                                          Number = c.Number,
                                          Name = c.Name,
                                          Instructor = c.Instructor,
                                          Location = c.Location,
                                          Days = c.Days,
                                          Time = c.Time,
                                          Department = d.Code
                                      };

                        CourseList = await courses.ToListAsync();

                        var assignments = from c in _context.Course
                                      join d in _context.Department on c.Department equals d.ID.ToString()
                                      join a in _context.Assignment on c.ID equals a.CourseID
                                      where c.InstructorID == UserID
                                      select new Assignment{
                                          ID = a.ID,
                                          Title = a.Title,
                                          Points = a.Points,
                                          Description = a.Description,
                                          Due = a.Due,
                                          SubmissionType = a.SubmissionType,
                                          CourseID = c.ID
                                      };
                        Assignments = await assignments.ToListAsync();

                        /*** Populate instructor to-do list ***/

                        List<Assignment> assignmentRecords;

                        List<Course> courseRecords;

                        AssignmentList = new List<Assignment>();

                        //Pulls records from db
                        courseRecords = _context.Course.Where(c => c.InstructorID == UserID).ToList();
                        assignmentRecords = _context.Assignment.ToList();

                        //Finds assignments for instructor courses
                        foreach (Course course in courseRecords)
                        {
                            foreach (Assignment assignment in assignmentRecords)
                            {
                                if (course.ID == assignment.CourseID)
                                {
                                    AssignmentList.Add(assignment);
                                }
                            }
                        }

                        OrderAssignments();

                        RetrieveCourseInformation(courseRecords);
                    }
                    else  //User is a student
                    {
                        /*** Populate student cards ***/

                        var courses = from c in _context.Course
                                      join d in _context.Department on c.Department equals d.ID.ToString()
                                      join r in _context.Registration on c.ID equals r.Course
                                      where r.Student == UserID

                                      select new Course
                                      {
                                          ID = c.ID,
                                          Number = c.Number,
                                          Name = c.Name,
                                          Instructor = c.Instructor,
                                          Location = c.Location,
                                          Days = c.Days,
                                          Time = c.Time,
                                          Department = d.Code
                                      };

                        CourseList = await courses.ToListAsync();

                        var assignments = from c in _context.Course
                                      join d in _context.Department on c.Department equals d.ID.ToString()
                                      join r in _context.Registration on c.ID equals r.Course
                                      join a in _context.Assignment on c.ID equals a.CourseID
                                      where r.Student == UserID
                                      select new Assignment{
                                          ID = a.ID,
                                          Title = a.Title,
                                          Points = a.Points,
                                          Description = a.Description,
                                          Due = a.Due,
                                          SubmissionType = a.SubmissionType,
                                          CourseID = c.ID
                                      };
                        Assignments = await assignments.ToListAsync();

                        /*** Populate student to-do list ***/

                        List<Assignment> assignmentRecords;

                        List<Registration> registrationRecords;

                        AssignmentList = new List<Assignment>();

                        //Pulls records from db
                        registrationRecords = _context.Registration.Where(r => r.Student == UserID).ToList();
                        assignmentRecords = _context.Assignment.ToList();

                        //Finds assignments for student
                        foreach (LMS.Models.Registration record in registrationRecords)
                        {
                            if (record.Student == UserID)  //Student is enrolled in course
                            {
                                foreach (Assignment assignment in assignmentRecords)
                                {
                                    if (assignment.CourseID == record.Course) //Assignment is assigned to course
                                    {
                                        AssignmentList.Add(assignment);
                                    }
                                }

                            }
                        }

                        OrderAssignments();

                        List<Course> courseRecords;
                        courseRecords = _context.Course.ToList();

                        RetrieveCourseInformation(courseRecords);
                    }
                    return Page();
                }
            }
            else
            {
                return new RedirectToPageResult("/Login");
            }
        }

        public async Task<IActionResult> OnPostAsync(int course)
        {
            HttpContext.Session.SetInt32("currCourse", course);

            string IsInstructor = HttpContext.Session.GetString("isInstructorSession");

            if (IsInstructor == "True")
            {
                return new RedirectToPageResult("./Courses/Assignments/Index");
            }
            else
            {
                return new RedirectToPageResult("/UserHome");
            }
        }

        /// <summary>
        /// Pulls course department code and number to display in to-do list
        /// </summary>
        /// <param name="courseRecords"></param>
        private void RetrieveCourseInformation(List<Course> courseRecords)
        {
            CourseInfo = new List<string>();

            
            foreach (Assignment assignment in AssignmentList)
            {
                string info = null;
                foreach (Course course in courseRecords)
                {
                    Department deptRecord = _context.Department.Where(d => d.ID.ToString() == course.Department).SingleOrDefault();
                    info = deptRecord.Code;
                }

                Course courseRecord = _context.Course.Where(c => c.ID == assignment.CourseID).SingleOrDefault();

                info += " " + courseRecord.Number;

                CourseInfo.Add(info);
            }
        }

        /// <summary>
        /// Orders a list or assignments by due date
        /// </summary>
        private void OrderAssignments()
        {
            //Order assignment dates
            AssignmentList = AssignmentList.OrderBy(a => a.Due).ToList();
            //Select top five assignments
            if (AssignmentList.Count > 5)
            {
                AssignmentList.RemoveRange(5, (AssignmentList.Count - 5));
            }
        }
    }
}