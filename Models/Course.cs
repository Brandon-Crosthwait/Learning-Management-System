using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models
{
    public class Course
    {
        public int ID { get; set; }

        [Required]
        public int Number { get; set; }

        [StringLength(60, MinimumLength = 1)]
        [Required]
        public string Name { get; set; }

        [StringLength(10, MinimumLength = 1)]
        [Required]
        public string Department { get; set; }

        [StringLength(1000, MinimumLength = 1)]
        [Required]
        public string Description { get; set; }

        [StringLength(60, MinimumLength = 1)]
        [Required]
        public string Instructor { get; set; }

        [StringLength(60, MinimumLength = 1)]
        [Required]
        public string Location { get; set; }

        [StringLength(10, MinimumLength = 1)]
        [Required]
        public string Days { get; set; }
        
        [DataType(DataType.Time)]
        [Required]
        public DateTime Time { get; set; }

        [Required]
        public int CreditHours { get; set; }

        [Required]
        public int Capacity { get; set; }

    }
}
