using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using LMS.Data;
using LMS.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace LMS.Pages.SignUp
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
        public User User { get; set; }
        public DateTime Birthday { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            int age;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Birthday = User.DateOfBirth;
            age = GetAge(Birthday);

            if (age < 18)
            {
                return Page();
            }

            // Password security using salt and hashing
            byte[] salt = new byte[128 / 8];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            User.Salt = Convert.ToBase64String(salt);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: User.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            User.Password = hashed; // users hashed password

            _context.User.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("../Index");
        }

        int GetAge(DateTime Birthday)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - Birthday.Year;

            if (Birthday > today.AddYears(-age))
                age--;

            return age;
        }
    }
}
