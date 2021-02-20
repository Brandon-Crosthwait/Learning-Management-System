using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace LMS.Pages
{
    public class LoginModel : PageModel
    {
        private readonly LMS.Data.LMSContext _context;

        public LoginModel(LMS.Data.LMSContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Grab the user from database using email(username)
            var userRecord = _context.User.Where(u => u.Email == Username).FirstOrDefault();

            if (userRecord != null)
            {
                // Grab user's salt and store in byte array
                byte[] salt = Convert.FromBase64String(userRecord.Salt);

                // Hash the user entered password
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: Password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

                // Check if hashed user entered password matches the user account password
                if (userRecord.Password == hashed)
                {
                    HttpContext.Session.SetInt32("userID", userRecord.ID);
                    HttpContext.Session.SetString("userFirstName", userRecord.FirstName);

                    return RedirectToPage("UserHome");
                }
                else
                {
                    return this.Page();
                }
            }
            else
            {
                return this.Page();
            }

            //if (Username.Equals("Garrett") && Password.Equals("12345"))
            //{
            //    HttpContext.Session.SetString("username", Username);

            //    return RedirectToPage("Index");
            //}
            //else
            //{
            //    return this.Page();
            //}
        }
    }
}
