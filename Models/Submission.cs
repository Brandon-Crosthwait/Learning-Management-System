using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Models
{
    public class Submission
    {
        public int ID { get; set; }

        [Required]
        public int AssignmentID { get; set; }

        [Required]
        public int StudentID { get; set; }

        [Required]
        public string SubmissionFile { get; set; }
    }
}
