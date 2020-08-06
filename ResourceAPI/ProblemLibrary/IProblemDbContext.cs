using Microsoft.EntityFrameworkCore;

namespace ProblemLibrary
{
    public interface IProblemDbContext
    {
        public DbSet<Problem> Problems { get; set; }

        public DbSet<Answer> Answers { get; set; }

        int SaveChanges(bool acceptAllChangesOnSuccess = true);
    }
}