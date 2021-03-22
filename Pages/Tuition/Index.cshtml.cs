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
           this._context = context;
        }

        public IList<Course> CourseList { get; set; }

        [BindProperty]
        public int UserID { get; set; }
        public User User { get; set; }

        [BindProperty]
        public int Amount { get; set; }

        public int cost {get;set;}

        public async Task OnGetAsync()
        {
            UserID = (int)HttpContext.Session.GetInt32("userID");
            User = _context.User.Where(u => u.ID == UserID).FirstOrDefault();

            Amount = 100;

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

            foreach (var item in CourseList)
            {
                cost += item.CreditHours * 100;
            }
            cost = cost - User.Payment;
        }

            public async Task<IActionResult> OnPostAsync(string stripeEmail, string stripeToken)
            {
                UserID = (int)HttpContext.Session.GetInt32("userID");
                User = _context.User.Where(u => u.ID == UserID).FirstOrDefault();
                User.Payment = Amount;
                await _context.SaveChangesAsync();

                var customers = new CustomerService();

                var customer = customers.Create(new CustomerCreateOptions {
                    Email = stripeEmail,
                    Source = stripeToken
                });

                var options = new ChargeCreateOptions {
                    Amount = Amount,
                    Currency = "usd",
                    Source = "tok_visa",
                    Description = "Tuition Payment",
                    Customer = customer.Id
                };
                
                return RedirectToPage("./Index");
            }
    }
}
