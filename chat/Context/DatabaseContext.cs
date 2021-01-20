using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using chat.Models;
using Microsoft.EntityFrameworkCore;

namespace chat.Context
{
    public class DatabaseContext : DbContext {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }

        public DbSet<Message> Messages { get; set; }
    }
}
