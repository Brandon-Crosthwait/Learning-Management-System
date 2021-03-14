using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models
{
    public class Assignment
    {
        public int ID { get; set; }

        [Required]
        public string Title { get; set; }
        
        [Required]
        public int Points { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Due { get; set; }

        [Required]
        public string SubmissionType { get; set; }

        [Required]
        public int CourseID { get; set; }
    }
}
