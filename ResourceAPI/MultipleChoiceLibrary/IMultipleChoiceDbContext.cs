using Microsoft.EntityFrameworkCore;

namespace MultipleChoiceLibrary
{
    public interface IMultipleChoiceDbContext
    {
        public DbSet<MultipleChoiceTest> MultipleChoiceTests { get; set; }
        public DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }
        public DbSet<MultipleChoiceAnswer> MultipleChoiceAnswers { get; set; }
        int SaveChanges(bool acceptAllChangesOnSuccess = true);
    }
}