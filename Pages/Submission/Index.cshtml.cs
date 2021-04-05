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

namespace LMS.Pages.Submission
{
    public class IndexModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;

        public IndexModel(LMS.Data.LMSContext context)
        {
            _context = context;
        }

        public IList<LMS.Models.Submission> Submission { get;set; }

        public IList<LMS.Models.User> Students { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                id = (int)HttpContext.Session.GetInt32("currAssignment");
            }
            Submission = await _context.Submission.Where(x => x.AssignmentID == id).ToListAsync();

            Students = await _context.User.Where(x => !x.IsInstructor).ToListAsync();

            return Page();
        }
    }
}
