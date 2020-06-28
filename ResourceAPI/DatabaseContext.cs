using Microsoft.EntityFrameworkCore;
using ResourceAPI.Models;

namespace ResourceAPI
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DatabaseContext()
        {
        }

        public DbSet<Answer> Answers { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Author> Authors { get; set; }

        public static string ConnectionString { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(ConnectionString);
            //Database.EnsureCreated();
        }
    }
}