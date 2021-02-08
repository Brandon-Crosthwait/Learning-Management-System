using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(60, MinimumLength = 8)]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        /// <summary>
        /// For password security
        /// </summary>
        public string Salt { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public Boolean IsInstructor { get; set; }
    }
}
