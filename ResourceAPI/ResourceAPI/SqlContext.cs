using System.Linq;
using CategoryLibrary;
using CommonLibrary;
using CommonLibrary.Interfaces;
using ExerciseLibrary;
using Microsoft.EntityFrameworkCore;
using ProblemLibrary;
using QuizLibrary;
using TagLibrary;
using VoteLibrary;

namespace ResourceAPI
{
    public class SqlContext : DbContext, IProblemDbContext, IExerciseDbContext, ICategoryDbContext,
        IQuizDbContext, IAuthorDbContext, IVoteDbContext
    {
        public SqlContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Comment> Comments { get; set; }

        //public DbSet<ProblemTag> ProblemTags { get; set; }

        //public DbSet<AnswerVote> AnswerVotes { get; set; }
        public static string FileDirectory { get; set; } = "../../images";
        public DbSet<Category> Categories { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Script> ExerciseScripts { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Quiz> QuizTests { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizAnswer> QuizAnswers { get; set; }
        public DbSet<VoteElement> Votes { get; set; }
        public DbSet<Author> Authors { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<ProblemTag>()
            //    .HasKey(pc => new {pc.TagUrl, pc.ProblemId});

            //modelBuilder.Entity<ProblemTag>()
            //    .HasOne(pc => pc.Problem)
            //    .WithMany(p => p.ProblemTags);

            //modelBuilder.Entity<ProblemTag>()
            //    .HasOne(category => category.Tag)
            //    .WithMany(tag => tag.ProblemTags)
            //    .HasForeignKey(pc => pc.TagUrl);

            //modelBuilder.Entity<ProblemTag>()
            //    .HasOne(pt => pt.Problem)
            //    .WithMany(p => p.ProblemTags)
            //    .HasForeignKey(pt => pt.ProblemId);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.Parent)
                .WithMany("Categories")
                .HasForeignKey("ParentId")
                .OnDelete(DeleteBehavior.Cascade)
                ;

            //modelBuilder.Entity<ProblemVote>().HasKey(pv => new {pv.ProblemId, pv.AuthorId});
            //modelBuilder.Entity<ProblemVote>().HasOne(pv => pv.Problem).WithMany(p => p.ProblemVotes);
            //modelBuilder.Entity<ProblemVote>().HasOne(pv => pv.Author).WithMany(a => a.Votes);

            //modelBuilder.Entity<AnswerVote>().HasKey(pv => new {pv.AnswerId, pv.AuthorId});
            //modelBuilder.Entity<AnswerVote>().HasOne(av => av.Answer).WithMany(a => a.AnswerVotes);
            //modelBuilder.Entity<AnswerVote>().HasOne(av => av.Author).WithMany(a => a.AnswerVotes);
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