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

namespace LMS.Pages.Courses.CourseInfo.Assignments
{
    public class EditModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;

        public EditModel(LMS.Data.LMSContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Assignment Assignment { get; set; }

        [BindProperty]
        public Course Course { get; set; }

        public SelectList SubmissionList { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<SelectListItem> types = new List<SelectListItem>();

            types.Add(new SelectListItem() { Text = "Text Box Entry", Value = "Text Box Entry" });
            types.Add(new SelectListItem() { Text = "File Upload", Value = "File Upload" });

            SubmissionList = new SelectList(types, "Value", "Text");

            Assignment = await _context.Assignment.FirstOrDefaultAsync(m => m.ID == id);

            if (Assignment == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            int courseID = (int)HttpContext.Session.GetInt32("currCourse");
            Course = await _context.Course.FirstOrDefaultAsync(m => m.ID == courseID);

            Assignment.CourseID = Course.ID;

            _context.Attach(Assignment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssignmentExists(Assignment.ID))
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

        private bool AssignmentExists(int id)
        {
            return _context.Assignment.Any(e => e.ID == id);
        }
    }
}
