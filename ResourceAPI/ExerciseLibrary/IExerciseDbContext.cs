using Microsoft.EntityFrameworkCore;

namespace ExerciseLibrary
{
    public interface IExerciseDbContext
    {
        public DbSet<Exercise> Exercises { get; set; }
        int SaveChanges(bool acceptAllChangesOnSuccess = true);
    }
}