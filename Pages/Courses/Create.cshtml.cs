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

namespace LMS.Pages.Courses
{
    public class CreateModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;

        public CreateModel(LMS.Data.LMSContext context)
        {
            _context = context;
        }

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

        public IActionResult OnGet()
        {
            if (HttpContext != null)
            {
                UserID = (int)HttpContext.Session.GetInt32("userID");
                if (UserID <= 0)
                {
                    return NotFound();
                }
                else
                {
                    PopulateDepartmentDropDownList(_context);
                    return Page();
                }
            }
            else
            {
                return NotFound();
            }
        }

        [BindProperty]
        public Course Course { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}
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

            Course.Instructor = User.LastName + ", " + User.FirstName;

            _context.Course.Add(Course);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
