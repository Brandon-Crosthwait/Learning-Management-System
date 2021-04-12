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

        public Assignment Assignment {get; set;}

        public int one;
        public int two;
        public int three;
        public int four;
        public int five;
        public int six;
        public int seven;
        public double placeholder;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                id = (int)HttpContext.Session.GetInt32("currAssignment");
            }
            Submission = await _context.Submission.Where(x => x.AssignmentID == id).ToListAsync();

            Students = await _context.User.Where(x => !x.IsInstructor).ToListAsync();

            one = 0;
            two = 0;
            three = 0;
            four = 0;
            five = 0;
            six = 0;
            seven = 0;

            foreach (var item in Submission)
            {
                Assignment = _context.Assignment.Where(u => u.ID == item.AssignmentID).FirstOrDefault();
                
                if(item.Grade != "--"){
                    placeholder = ((double.Parse(item.Grade)/Assignment.Points)*100);
                    if(placeholder >= 95){
                        seven = seven + 1;
                    }
                    else if(placeholder >= 90){
                        six = six + 1;
                    }
                    else if(placeholder >= 85){
                        five = five + 1;
                    }
                    else if(placeholder >= 80){
                        four = four + 1;
                    }
                    else if(placeholder >= 75){
                        three = three + 1;
                    }
                    else if(placeholder >= 70){
                        two = two + 1;
                    }
                    else if(placeholder < 70){
                        one = one + 1;
                    }
                }
            }

            return Page();
        }
    }
}
