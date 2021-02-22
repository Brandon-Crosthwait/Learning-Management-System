using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace LMS.Pages.Profile
{
    public class EditModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public EditModel(LMS.Data.LMSContext context,
                         IWebHostEnvironment webHostEnvironment)
        {
            this._context = context;
            this.webHostEnvironment = webHostEnvironment;
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
        [BindProperty]
        public IFormFile Photo { get; set; }
        public IWebHostEnvironment WebHostEnvironment { get; }

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
                if (Photo != null)
                {
                    if (User.PhotoPath != null)
                    {
                        string filePath = Path.Combine(webHostEnvironment.WebRootPath,
                        "images", User.PhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    User.PhotoPath = ProcessUploadedFile();
                }

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

        private string ProcessUploadedFile()
        {
            string uniqueFileName = null;

            if (Photo != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
