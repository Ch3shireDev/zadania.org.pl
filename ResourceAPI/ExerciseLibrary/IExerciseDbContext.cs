using Microsoft.EntityFrameworkCore;

namespace ExerciseLibrary
{
    public interface IExerciseDbContext
    {
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Script> ExerciseScripts { get; set; }
        int SaveChanges(bool acceptAllChangesOnSuccess = true);
    }
}