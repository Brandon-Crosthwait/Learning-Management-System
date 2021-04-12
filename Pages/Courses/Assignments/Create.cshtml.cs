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

namespace LMS.Pages.Courses.CourseInfo.Assignments
{
    public class CreateModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;

        public CreateModel(LMS.Data.LMSContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Course Course { get; set; }

        public SelectList SubmissionList { get; set; }

        public IActionResult OnGet()
        {
            List<SelectListItem> types = new List<SelectListItem>();

            types.Add(new SelectListItem() { Text = "Text Box Entry", Value = "Text Box Entry" });
            types.Add(new SelectListItem() { Text = "File Upload", Value = "File Upload" });

            SubmissionList = new SelectList(types, "Value", "Text");

            return Page();
        }

        [BindProperty]
        public Assignment Assignment { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            int courseID = (int)HttpContext.Session.GetInt32("currCourse");
            Course = await _context.Course.FirstOrDefaultAsync(m => m.ID == courseID);

            Assignment.CourseID = Course.ID;

            _context.Assignment.Add(Assignment);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
