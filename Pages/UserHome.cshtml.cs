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

        #region Attributes
        /// <summary>
        /// Username of current User
        /// </summary>
        public string Username;

        /// <summary>
        /// UserID of current User
        /// </summary>
        public int UserID;

        /// <summary>
        /// Instance of a User
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// A list of Course objects
        /// </summary>
        public IList<Course> CourseList { get; set; }

        /// <summary>
        /// A list of Assignment objects
        /// </summary>
        public List<Assignment> AssignmentList { get; set; }

        /// <summary>
        /// A list of notification objects.
        /// </summary>
        public List<Notification> NotificationList { get; set; }

        /// <summary>
        /// Secondary list of Assignment objects
        /// </summary>
        public List<Assignment> Assignments { get; set; }

        /// <summary>
        /// A list of pulled Course information
        /// </summary>
        public List<string> CourseInfo { get; set; }
        #endregion

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
                    string IsInstructor = HttpContext.Session.GetString("isInstructorSession");

                    if (IsInstructor == "True")
                    {
                        await PopulateInstructorCards();

                        PopulateInstructorToDoList();

                        PopulateInstructorNotifications();
                    }
                    else  //User is a student
                    {
                        await PopulateStudentCards();

                        PopulateStudentToDoList();

                        PopulateStudentNotifications();
                    }
                    return Page();
                }
            }
            else
            {
                return new RedirectToPageResult("/Login");
            }
        }

        /// <summary>
        /// Pulls instructor assignments from the db and displays each assignment in the to-do list
        /// </summary>
        private void PopulateInstructorToDoList()
        {
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

        /// <summary>
        /// Pulls instructor courses from the db and displays each course on cards
        /// </summary>
        /// <returns>Task</returns>
        private async Task PopulateInstructorCards()
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
                              select new Assignment
                              {
                                  ID = a.ID,
                                  Title = a.Title,
                                  Points = a.Points,
                                  Description = a.Description,
                                  Due = a.Due,
                                  SubmissionType = a.SubmissionType,
                                  CourseID = c.ID
                              };
            Assignments = await assignments.ToListAsync();
        }

        private async Task PopulateInstructorNotifications()
        {
            NotificationList = new List<Notification>();
            NotificationList = _context.Notification.Where(n => n.StudentID == UserID).ToList();

            if (NotificationList.Count > 2)
            {
                NotificationList.RemoveRange(2, (NotificationList.Count - 2));
            }
        }

        /// <summary>
        /// Pulls student courses from the db and displays each course on cards
        /// </summary>
        /// <returns>Task</returns>
        private async Task PopulateStudentCards()
        {
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
                              select new Assignment
                              {
                                  ID = a.ID,
                                  Title = a.Title,
                                  Points = a.Points,
                                  Description = a.Description,
                                  Due = a.Due,
                                  SubmissionType = a.SubmissionType,
                                  CourseID = c.ID
                              };
            Assignments = await assignments.ToListAsync();
        }

        /// <summary>
        /// Pulls student assignments from the db and displays each assignment in the to-do list
        /// </summary>
        private void PopulateStudentToDoList()
        {
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

        /// <summary>
        /// Pulls student notifications from the db and stores each notification in the notificaiton box
        /// </summary>
        private void PopulateStudentNotifications()
        {
            NotificationList = new List<Notification>();
            NotificationList = _context.Notification.Where(n => n.StudentID == UserID).ToList();

            if (NotificationList.Count > 2)
            {
                NotificationList.RemoveRange(2, (NotificationList.Count - 2));
            }

            if (NotificationList.Count == 0)
            {
                Notification notification = new Notification();
                notification.Message = "No new notifications.";
                NotificationList.Add(notification);
            }
        }

        /// <summary>
        /// Pulls course department code and number to display in to-do list
        /// </summary>
        /// <param name="courseRecords"></param>
        private void RetrieveCourseInformation(List<Course> courseRecords)
        {
            CourseInfo = new List<string>();

            string info = null;

            //Pulls course number and department to display in to do list
            foreach (Assignment assignment in AssignmentList)
            {
                foreach (Course course in courseRecords)
                {
                    if (assignment.CourseID == course.ID)
                    {
                        Department deptRecord = _context.Department.Where(d => d.ID.ToString() == course.Department).FirstOrDefault();
                        info = deptRecord.Code;
                        info += " " + course.Number;
                    }
                }
                CourseInfo.Add(info);
            }
        }

        /// <summary>
        /// Orders a list of assignments by due date
        /// </summary>
        private void OrderAssignments()
        {
            DateTime currentDateTime = DateTime.Now;

            //Remove past assignments
            for (int i = 0; i < AssignmentList.Count; i++)
            {
                if (AssignmentList[i].Due < currentDateTime)
                {
                    AssignmentList.RemoveAt(i);
                    i = -1;
                }
            }

            //Order assignments by date
            AssignmentList = AssignmentList.OrderBy(a => a.Due).ToList();
            //Select top five assignments
            if (AssignmentList.Count > 5)
            {
                AssignmentList.RemoveRange(5, (AssignmentList.Count - 5));
            }
        }

        public async Task<IActionResult> OnPostAsync(int course, int assignment)
        {
            string IsInstructor = HttpContext.Session.GetString("isInstructorSession");

            // User clicks on Course card and is sent to Assignments page for that Course
            if (course != 0)
            {
                HttpContext.Session.SetInt32("currCourse", course);

                return new RedirectToPageResult("./Courses/Assignments/Index");
            }

            // User clicks on ToDo List item and is sent to the Submission page for that Assignment
            if (assignment != 0)
            {
                HttpContext.Session.SetInt32("currAssignment", assignment);

                // If Student, the User is sent to Submission/Create
                // If Instructor, the User is sent to the list of Submissions for Assignment
                if (IsInstructor == "False")
                {
                    return new RedirectToPageResult("./Submission/Create");
                }
                else
                {
                    return new RedirectToPageResult("./Submission/Index");
                }
            }

            return new RedirectToPageResult("/UserHome");
        }
    }
}