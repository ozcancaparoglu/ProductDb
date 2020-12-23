using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Data.OnnetDb
{
    public class OnnetDbContext : DbContext
    {
        public DbSet<OnnetProduct> Product { get; set; }
        public OnnetDbContext(DbContextOptions<OnnetDbContext> options) : base(options)
        {
        }
    }
}
