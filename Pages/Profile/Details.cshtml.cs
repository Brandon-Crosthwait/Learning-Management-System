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

namespace LMS.Pages.Profile
{
    public class DetailsModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;

        public DetailsModel(LMS.Data.LMSContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int UserID { get; set; }
        public User User { get; set; }
        public string Link1 { get; set; }
        public string Link2 { get; set; }
        public string Link3 { get; set; }

        public IActionResult OnGet()
        {
            // Grab the ID of the user who logged in
            if (HttpContext != null)
            {
                UserID = (int)HttpContext.Session.GetInt32("userID");
                if (UserID <= 0)
                {
                    return new RedirectToPageResult("/Login");
                }
                else
                {
                    User = _context.User.Where(u => u.ID == UserID).FirstOrDefault();
                    Link1 = User.Link1;
                    Link2 = User.Link2;
                    Link3 = User.Link3;
                    return Page();
                }
            }
            else
            {
                return new RedirectToPageResult("/Login");
            }
        }

        public IActionResult OnPost()
        {
            HttpContext.Session.Clear();
            return new RedirectToPageResult("../Index");
        }

    }

}
