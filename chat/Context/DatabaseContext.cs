using chat.Models;
using Microsoft.EntityFrameworkCore;

namespace chat.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}