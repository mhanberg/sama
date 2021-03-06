﻿using Microsoft.EntityFrameworkCore;
using sama.Models;

namespace sama
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Endpoint> Endpoints { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Setting> Settings { get; set; }
    }
}
