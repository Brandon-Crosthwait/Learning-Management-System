using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models
{
    public class Department
    {
        public int ID { get; set; }

        [StringLength(60, MinimumLength = 1)]
        [Required]
        public string Name { get; set; }

        [StringLength(10, MinimumLength = 1)]
        [Required]
        public string Code { get; set; }
    }
}
