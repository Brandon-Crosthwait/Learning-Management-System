using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LMS.Data;
using LMS.Models;

namespace LMS.Pages.Notifications
{
    public class IndexModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;

        public IndexModel(LMS.Data.LMSContext context)
        {
            _context = context;
        }

        public IList<Notification> Notification { get;set; }

        public async Task OnGetAsync()
        {
            Notification = await _context.Notification.ToListAsync();
        }
    }
}
