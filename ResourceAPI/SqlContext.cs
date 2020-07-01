using Microsoft.EntityFrameworkCore;
using ResourceAPI.Models;

namespace ResourceAPI
{
    public class SqlContext : DbContext
    {
        public SqlContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Answer> Answers { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProblemTag> ProblemTags { get; set; }

        public DbSet<ProblemVote> ProblemVotes { get; set; }
        public DbSet<AnswerVote> AnswerVotes { get; set; }

        public static string ConnectionString { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProblemTag>()
                .HasKey(pc => new {pc.TagUrl, pc.ProblemId});

            modelBuilder.Entity<ProblemTag>()
                .HasOne(pc => pc.Problem)
                .WithMany(p => p.ProblemTags);

            modelBuilder.Entity<ProblemTag>()
                .HasOne(category => category.Tag)
                .WithMany(tag => tag.ProblemCategories)
                .HasForeignKey(pc => pc.TagUrl);

            modelBuilder.Entity<ProblemTag>()
                .HasOne(pt => pt.Problem)
                .WithMany(p => p.ProblemTags)
                .HasForeignKey(pt => pt.ProblemId);

            modelBuilder.Entity<ProblemVote>().HasKey(pv => new {pv.ProblemId, pv.AuthorId});
            modelBuilder.Entity<ProblemVote>().HasOne(pv => pv.Problem).WithMany(p => p.ProblemVotes);
            modelBuilder.Entity<ProblemVote>().HasOne(pv => pv.Author).WithMany(a => a.ProblemVotes);

            modelBuilder.Entity<AnswerVote>().HasKey(pv => new {pv.AnswerId, pv.AuthorId});
            modelBuilder.Entity<AnswerVote>().HasOne(av => av.Answer).WithMany(a => a.AnswerVotes);
            modelBuilder.Entity<AnswerVote>().HasOne(av => av.Author).WithMany(a => a.AnswerVotes);
        }
    }
}