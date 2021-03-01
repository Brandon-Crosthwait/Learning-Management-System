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

namespace LMS.Pages.Courses
{
    public class EditModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;

        public EditModel(LMS.Data.LMSContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Course Course { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        [BindProperty]
        public bool Monday { get; set; }
        [BindProperty]
        public bool Tuesday { get; set; }
        [BindProperty]
        public bool Wednesday { get; set; }
        [BindProperty]
        public bool Thursday { get; set; }
        [BindProperty]
        public bool Friday { get; set; }

        [BindProperty]
        public DateTime StartTime { get; set; }
        [BindProperty]
        public DateTime EndTime { get; set; }

        public SelectList DepartmentList { get; set; }

        public void PopulateDepartmentDropDownList(LMSContext _context,
            object selectedDepartment = null)
        {
            var departmentsQuery = from d in _context.Department
                                   orderby d.Name // Sort by name.
                                   select d;

            DepartmentList = new SelectList(departmentsQuery.AsNoTracking(),
                        "ID", "Code", selectedDepartment);
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {


            if (HttpContext != null)
            {
                UserID = (int)HttpContext.Session.GetInt32("userID");
                if (UserID <= 0)
                {
                    return new RedirectToPageResult("/Login");
                }
                else
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    PopulateDepartmentDropDownList(_context);
                    Course = await _context.Course.FirstOrDefaultAsync(m => m.ID == id);

                    string startTime = Course.Time.Substring(0, Course.Time.IndexOf('-') - 1);
                    string endTime = Course.Time.Substring(Course.Time.LastIndexOf('-') + 1);

                    StartTime = Convert.ToDateTime(startTime);
                    EndTime = Convert.ToDateTime(endTime);

                    string thSubS = "Th";

                    if (Course.Days.Contains('M'))
                    {
                        Monday = true;
                    }
                    if (Course.Days.Contains('T'))
                    {
                        Tuesday = true;
                    }
                    if (Course.Days.Contains('W'))
                    {
                        Wednesday = true;
                    }
                    if (Course.Days.Contains(thSubS))
                    {
                        Thursday = true;
                    }
                    if (Course.Days.Contains('F'))
                    {
                        Friday = true;
                    }

                    if (Course == null)
                    {
                        return NotFound();
                    }

                    return Page();
                }
            }
            else
            {
                return new RedirectToPageResult("/Login");
            }
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            _context.Attach(Course).State = EntityState.Modified;

            UserID = (int)HttpContext.Session.GetInt32("userID");
            User = _context.User.Where(u => u.ID == UserID).FirstOrDefault();

            if (Monday)
            {
                Course.Days += "M";
            }
            if (Tuesday)
            {
                Course.Days += "T";
            }
            if (Wednesday)
            {
                Course.Days += "W";
            }
            if (Thursday)
            {
                Course.Days += "Th";
            }
            if (Friday)
            {
                Course.Days += "F";
            }

            Course.Time = StartTime.ToString("h:mmtt") + " - " + EndTime.ToString("h:mmtt");

            Course.Instructor = User.LastName + ", " + User.FirstName;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(Course.ID))
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

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.ID == id);
        }
    }
}
