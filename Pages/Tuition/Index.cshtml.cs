﻿using System;
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
using Newtonsoft.Json.Linq;

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
        public string CardNumber { get; set; }

        [BindProperty]
        public string CVV { get; set; }
        
        [BindProperty]
        public string Month { get; set; }
        
        [BindProperty]
        public string Year { get; set; }

        public int cost {get;set;}
        
        [BindProperty]
        public LMS.Models.Submission Submission { get; set; }

        public Assignment Assignment {get; set;}

        public int one;
        public int two;
        public int three;
        public int four;
        public int five;
        public int six;
        public int seven;
        
        public int placeholder;
        public int points;

        public async Task OnGetAsync()
        {
            UserID = (int)HttpContext.Session.GetInt32("userID");
            User = _context.User.Where(u => u.ID == UserID).FirstOrDefault();

            Amount = 100;
            one = 0;
            two = 0;
            three = 0;
            four = 0;
            five = 0;
            six = 0;
            seven = 0;

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

            List<LMS.Models.Submission> SubmissionsByAssignment = new List<LMS.Models.Submission>();
            SubmissionsByAssignment = _context.Submission.ToList();

            foreach (var item in SubmissionsByAssignment)
            {
                Assignment = _context.Assignment.Where(u => u.ID == item.AssignmentID).FirstOrDefault();
                points = Assignment.Points;
                
                if(item.Grade != "--"){
                    placeholder = (Int32.Parse(item.Grade)/points)*100;
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
        }

            public async Task<IActionResult> OnPostAsync()
            {
                //grab user
                UserID = (int)HttpContext.Session.GetInt32("userID");
                User = _context.User.Where(u => u.ID == UserID).FirstOrDefault();
                
                //initalize httpClient
                var httpClient = new HttpClient();

                //Get Card Token
                var token = new HttpRequestMessage(new HttpMethod("POST"), "https://api.stripe.com/v1/tokens");
                token.Headers.TryAddWithoutValidation("Authorization", "Bearer pk_test_51IV89XFlShtBVarWzUaU7rhtEQPlJi1wnxgTSOm2SZjuAIum2cc3oCuhEeL1FWTb2OzfjNo4fwZ6rDf98A3mlpkJ00DSY8nmLC"); 
                token.Content = new StringContent("card[number]="+CardNumber+"&card[exp_month]="+Month+"&card[exp_year]="+Year+"&card[cvc]="+CVV);
                token.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded"); 
                var response = await httpClient.SendAsync(token);
                var contents = await response.Content.ReadAsStringAsync();

                //If token is valid update payment in database
                JObject tok = JObject.Parse(contents);
                string tokenID = (string)tok["id"];
                try{
                    if(tokenID.StartsWith("tok")){
                        User.Payment = User.Payment + Amount;
                    }
                }
                catch{
                    //payment not valid
                }

                //Make Charge
                var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.stripe.com/v1/charges");
                request.Headers.TryAddWithoutValidation("Authorization", "Bearer sk_test_51IV89XFlShtBVarWOf9WuHB2ra8HWdm3jpXc6VsrKqzpLAHpZkyculiBARpDmSfQGxhSjNmi5s82U2I0Ly58aaau005UGfGipY");                     request.Content = new StringContent("amount="+Amount*100+"&currency=usd&source="+tokenID);
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded"); 
                var checkout = await httpClient.SendAsync(request);             
                
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
    }
}
