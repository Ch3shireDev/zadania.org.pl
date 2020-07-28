using Microsoft.EntityFrameworkCore;
using ResourceAPI.Models.Exercise;

namespace ResourceAPI
{
    public interface IExerciseDbContext
    {
        public DbSet<Exercise> Exercises { get; set; }
    }
}