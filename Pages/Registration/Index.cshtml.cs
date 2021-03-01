  
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

namespace LMS.Pages.Registrations
{
    public class IndexModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;

        public IndexModel(LMS.Data.LMSContext context)
        {
            _context = context;
        }

        public IList<Course> Course { get;set; }
        public IList<Registration> RegList{ get;set; }
        public string registerButton = "Register";

        [BindProperty]
        public int UserID { get; set; }
        public User User { get; set; }
        
        [BindProperty]
        public Registration Registration {get; set;}

        public async Task OnGetAsync()
        {
            UserID = (int)HttpContext.Session.GetInt32("userID");
            Course = await _context.Course.ToListAsync();
            RegList = _context.Registration.Where(u => u.Student == UserID).ToList();
        }

        public async Task<IActionResult> OnPostAsync(int course)
        {
            UserID = (int)HttpContext.Session.GetInt32("userID");
            RegList = _context.Registration.Where(u => u.Student == UserID).ToList();
            var currRegistration = RegList.Where(u => u.Course == course).FirstOrDefault();
            if(currRegistration != null){
                _context.Registration.Remove(currRegistration);
            }
            else{
                Registration.Course = course;
                Registration.Student = UserID;
                _context.Registration.Add(Registration);
            }
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");               
        }
    }
}