using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

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

        [StringLength(60, MinimumLength = 0)]
        public string Address { get; set; }

        [StringLength(60, MinimumLength = 0)]
        public string City { get; set; }

        [StringLength(60, MinimumLength = 0)]
        public string State { get; set; }

        [StringLength(10, MinimumLength = 0)]
        public string Zip { get; set; }

        [StringLength(60, MinimumLength = 0)]
        public string PhoneNumber { get; set; }

        [RegularExpression(@"^([\w-\.]+@([\w-]+\.)+[\w-]{2,6})?$")]
        [StringLength(255, MinimumLength = 1)]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(1000, MinimumLength = 0)]
        public string BioInfo { get; set; }

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

        public string PhotoPath { get; set; }

        public string Link1 { get; set; }
        public string Link2 { get; set; }
        public string Link3 { get; set; }

        public int Payment { get; set; }
    }
}
