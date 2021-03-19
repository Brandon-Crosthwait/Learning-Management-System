using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using LMS.Data;
using LMS.Models;

namespace LMS.Pages.Submission
{
    public class CreateModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;

        public CreateModel(LMS.Data.LMSContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public LMS.Models.Submission Submission { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Submission.Add(Submission);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
