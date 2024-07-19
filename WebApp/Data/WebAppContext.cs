﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Data
{
    public class WebAppContext : DbContext
    {
        public WebAppContext (DbContextOptions<WebAppContext> options)
            : base(options)
        {
        }

        public DbSet<WebApp.Models.Category> Category { get; set; } = default!;
        public DbSet<WebApp.Models.ApplicationType> ApplicationType { get; set; } = default!;
        public DbSet<WebApp.Models.Product> Product { get; set; } = default!;
        public DbSet<WebApp.Models.ArticleModel> Articles { get; set; } = default!;
        public DbSet<WebApp.Models.ImageModel> Images { get; set; } = default!;
    }
}
