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
    }
}
