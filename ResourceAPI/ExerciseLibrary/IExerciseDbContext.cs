using Microsoft.EntityFrameworkCore;

namespace ExerciseLibrary
{
    public interface IExerciseDbContext
    {
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<ExerciseVariableData> ExerciseVariablesData { get; set; }
        public DbSet<ExerciseAnswerData> ExerciseAnswersData { get; set; }
        int SaveChanges(bool acceptAllChangesOnSuccess = true);
    }
}