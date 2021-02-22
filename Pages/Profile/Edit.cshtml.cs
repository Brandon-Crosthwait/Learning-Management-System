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

namespace LMS.Pages.Profile
{
    public class EditModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;

        public EditModel(LMS.Data.LMSContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; }
        [BindProperty]
        public int UserID { get; set; }
        [BindProperty]
        public string FirstName { get; set; }
        [BindProperty]
        public string LastName { get; set; }
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Link1 { get; set; }
        [BindProperty]
        public string Link2 { get; set; }
        [BindProperty]
        public string Link3 { get; set; }


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

                    FirstName = User.FirstName;
                    LastName = User.LastName;
                    Email = User.Email;
                    Link1 = User.Link1;
                    Link2 = User.Link2;
                    Link3 = User.Link3;

                    return Page();
                }
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            UserID = (int)HttpContext.Session.GetInt32("userID");
            User = _context.User.Where(u => u.ID == UserID).FirstOrDefault();

            try
            {
                User.FirstName = FirstName;
                User.LastName = LastName;
                User.Email = Email;
                User.Link1 = Link1;
                User.Link2 = Link2;
                User.Link3 = Link3;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToPage("./Details");
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.ID == id);
        }
    }
}
