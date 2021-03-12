using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LMS.Data;
using LMS.Models;

namespace LMS.Pages.Courses.CourseInfo.Assignments
{
    public class DetailsModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;

        public DetailsModel(LMS.Data.LMSContext context)
        {
            _context = context;
        }

        public Assignment Assignment { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Assignment = await _context.Assignment.FirstOrDefaultAsync(m => m.ID == id);

            if (Assignment == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
