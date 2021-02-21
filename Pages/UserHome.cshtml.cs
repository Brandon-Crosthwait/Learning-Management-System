using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using LMS.Models;

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

        public IActionResult OnGet()
        {
            // Grab the ID of the user who logged in
            if (HttpContext != null)
            {
                UserID = (int)HttpContext.Session.GetInt32("userID");
                if (UserID <= 0)
                {
                    return NotFound();
                }
                else
                {
                    User = _context.User.Where(u => u.ID == UserID).FirstOrDefault();
                    return Page();
                }
            }
            else
            {
                return NotFound();
            }
        }
    }
}
