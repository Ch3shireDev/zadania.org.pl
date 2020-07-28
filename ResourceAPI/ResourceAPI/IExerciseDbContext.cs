using ExerciseLibrary;
using Microsoft.EntityFrameworkCore;

namespace ResourceAPI
{
    public interface IExerciseDbContext
    {
        public DbSet<Exercise> Exercises { get; set; }
    }
}