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

        //public async Task<IActionResult> OnGetAsync(int? id)
        //{
        //    if (HttpContext != null)
        //    {
        //        UserID = (int)HttpContext.Session.GetInt32("userID");
        //    }
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    User = await _context.User.FirstOrDefaultAsync(m => m.ID == UserID);

        //    if (User == null)
        //    {
        //        return NotFound();
        //    }
        //    return Page();
        //}
    }
}
