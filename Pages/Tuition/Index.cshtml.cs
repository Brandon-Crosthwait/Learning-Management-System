using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;
using Stripe.Checkout;


namespace LMS.Pages.Tuition
{
    public class IndexModel : PageModel
    {
        
        private readonly LMS.Data.LMSContext _context;

        public IndexModel(LMS.Data.LMSContext context)
        {
            _context = context;
        }

        public IList<Course> CourseList { get; set; }

        [BindProperty]
        public int UserID { get; set; }

        public async Task OnGetAsync()
        {
            UserID = (int)HttpContext.Session.GetInt32("userID");
            var courses = from c in _context.Course
                          join r in _context.Registration on c.ID equals r.Course
                          where r.Student == UserID

                          select new Course
                          {
                              ID = c.ID,
                              Number = c.Number,
                              Name = c.Name,
                              Instructor = c.Instructor,
                              Location = c.Location,
                              Days = c.Days,
                              Time = c.Time,
                              CreditHours = c.CreditHours,
                          };

            CourseList = await courses.ToListAsync();
        }
        
            public IActionResult Charge(string  stripeEmail, string stripeToken)
            {
                var customers = new CustomerService();
                var charges = new ChargeService();

                var customer = customers.Create(new CustomerCreateOptions {
                    Email = stripeEmail,
                    Source = stripeToken
                });

                var charge = charges.Create(new ChargeCreateOptions {
                    Amount = 500,
                    Description = "Sample Charge",
                    Currency = "usd",
                    Customer = customer.Id
                });

                return Page();
            }
            
            public IActionResult Index()
            {
                return Page();
            }

            public IActionResult Error()
            {
                return Page();
            }
    }
}
