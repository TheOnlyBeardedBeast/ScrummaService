using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ScrummaService.Models;

namespace ScrummaService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Group> Groups { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
