using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LMS.Models;

namespace LMS.Data
{
    public class LMSContext : DbContext
    {
        public LMSContext (DbContextOptions<LMSContext> options)
            : base(options)
        {
        }

        public DbSet<LMS.Models.User> User { get; set; }

        public DbSet<LMS.Models.Course> Course { get; set; }

        public DbSet<LMS.Models.Department> Department { get; set; }

        public DbSet<LMS.Models.Registration> Registration { get; set; }

        public DbSet<LMS.Models.Assignment> Assignment { get; set; }

        public DbSet<LMS.Models.Submission> Submission { get; set; }

        public DbSet<LMS.Models.Notification> Notification { get; set; }
    }
}
