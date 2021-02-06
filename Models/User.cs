using System;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
    public class User
    {
        public int ID { get; set; }

        [StringLength(60, MinimumLength = 1)]
        [Required]
        public string FirstName { get; set; }

        [StringLength(60, MinimumLength = 1)]
        [Required]
        public string LastName { get; set; }

        [RegularExpression(@"^([\w-\.]+@([\w-]+\.)+[\w-]{2,6})?$")]
        [StringLength(255, MinimumLength = 1)]
        [Required]
        public string Email { get; set; }

        [StringLength(60, MinimumLength = 8)]
        [Required]
        public string Password { get; set; }


        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public Boolean IsInstructor { get; set; }
    }
}
