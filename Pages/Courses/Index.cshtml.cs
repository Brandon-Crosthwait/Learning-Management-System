using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Http;

namespace LMS.Pages.Courses
{
    public class IndexModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;

        public IndexModel(LMS.Data.LMSContext context)
        {
            _context = context;
        }

        public IList<Course> Courses { get;set; }

        public int UserID { get; set; }
        public User User { get; set; }

        public IActionResult OnGet()
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
                    UserID = (int)HttpContext.Session.GetInt32("userID");
                    User = _context.User.Where(u => u.ID == UserID).FirstOrDefault();
                    string instructor = User.LastName + ", " + User.FirstName;
                    Courses = _context.Course.Where(u => u.Instructor == instructor).ToList();
                    return Page();
                }
            }
            else
            {
                return new RedirectToPageResult("/Login");
            }
        }

        //public async Task OnGetAsync()
        //{
        //    UserID = (int)HttpContext.Session.GetInt32("userID");
        //    User = _context.User.Where(u => u.ID == UserID).FirstOrDefault();
        //    string instructor = User.LastName + ", " + User.FirstName;
        //    Course = await _context.Course.Where(u => u.Instructor == instructor).ToListAsync();
        //}
    }
}
