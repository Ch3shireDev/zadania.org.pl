using System;
using System.Linq;
using CategoryLibrary;
using CommonLibrary;
using ExerciseLibrary;
using FileDataLibrary;
using Microsoft.EntityFrameworkCore;
using ProblemLibrary;
using QuizLibrary;

namespace ResourceAPI
{
    public class SqlContext : DbContext, IProblemDbContext, IExerciseDbContext, ICategoryDbContext,
        IQuizDbContext, IAuthorDbContext, IFileDataDbContext
    {
        public SqlContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Script> ExerciseScripts { get; set; }
        public DbSet<FileData> FileData { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Quiz> QuizTests { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizAnswer> QuizAnswers { get; set; }

        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is Post && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((Post) entityEntry.Entity).Edited = DateTime.Now;

                if (entityEntry.State == EntityState.Added) ((Post) entityEntry.Entity).Created = DateTime.Now;
            }

            return base.SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Parent)
                .WithMany("Categories")
                .HasForeignKey("ParentId")
                .OnDelete(DeleteBehavior.Cascade)
                ;
        }

        public void Initialize()
        {
            if (!Categories.Any()) Categories.Add(new Category {Name = "Root"});
            SaveChanges();
            if (!Authors.Any()) Authors.Add(new Author {Name = "Administrator"});
            SaveChanges();
            if (!Problems.Any())
                Problems.Add(new Problem {Name = "abc", Content = "cde", CategoryId = 1, AuthorId = 1});
            SaveChanges();
        }
    }
}