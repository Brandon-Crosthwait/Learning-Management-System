using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace LMS.Pages
{
    public class UserHomeModel : PageModel
    {
        public string Username;
        public void OnGet()
        {
            // Grab the name of the user who logged in
            if (HttpContext != null)
            {
                Username = HttpContext.Session.GetString("userFirstName");
            }
        }
    }
}
