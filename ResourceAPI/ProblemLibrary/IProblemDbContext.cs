using Microsoft.EntityFrameworkCore;

namespace ProblemLibrary
{
    public interface IProblemDbContext
    {
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public DbSet<ProblemVote> ProblemVotes { get; set; }

        int SaveChanges(bool acceptAllChangesOnSuccess = true);
    }
}