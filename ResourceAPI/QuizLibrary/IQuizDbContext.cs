using Microsoft.EntityFrameworkCore;

namespace QuizLibrary
{
    public interface IQuizDbContext
    {
        public DbSet<Quiz> QuizTests { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizAnswer> QuizAnswers { get; set; }
        int SaveChanges(bool acceptAllChangesOnSuccess = true);
    }
}