using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models
{
    public class Registration
    {
        public int ID { get; set; }

        [Required]
        public int CourseID { get; set; }

        [Required]
        public int UserID { get; set; }

    }
}
