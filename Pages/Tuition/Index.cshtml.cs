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
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;
using Newtonsoft.Json;

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
        
        [BindProperty]
        public int CardNumber { get; set; }

        [BindProperty]
        public int CVV { get; set; }
        
        [BindProperty]
        public int Expiration { get; set; }

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

            public async Task<IActionResult> OnPostAsync()
            {
                UserID = (int)HttpContext.Session.GetInt32("userID");
                User = _context.User.Where(u => u.ID == UserID).FirstOrDefault();
                User.Payment = User.Payment - Amount;
                await _context.SaveChangesAsync();
                
                var httpClient = new HttpClient();

                var token = new HttpRequestMessage(new HttpMethod("POST"), "https://api.stripe.com/v1/tokens");
                token.Headers.TryAddWithoutValidation("Authorization", "Bearer pk_test_51IV89XFlShtBVarWzUaU7rhtEQPlJi1wnxgTSOm2SZjuAIum2cc3oCuhEeL1FWTb2OzfjNo4fwZ6rDf98A3mlpkJ00DSY8nmLC"); 
                token.Content = new StringContent("card[number]=4242424242424242&card[exp_month]=3&card[exp_year]=2022&card[cvc]=314");
                token.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded"); 
                var response = await httpClient.SendAsync(token);
                
                var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.stripe.com/v1/charges");
                request.Headers.TryAddWithoutValidation("Authorization", "Bearer sk_test_51IV89XFlShtBVarWOf9WuHB2ra8HWdm3jpXc6VsrKqzpLAHpZkyculiBARpDmSfQGxhSjNmi5s82U2I0Ly58aaau005UGfGipY"); 
                request.Content = new StringContent("amount=2000&currency=usd&source=tok_1IXxKZFlShtBVarWPR7jB3aQ&description=test");
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded"); 
                var checkout = await httpClient.SendAsync(request);

                return RedirectToPage("./Index");
            }
    }
}
