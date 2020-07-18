using System.Collections.Generic;
using System.Linq;
using ResourceAPI.Models.Exercise;

namespace ResourceAPI.ApiServices
{
    public interface IExerciseService
    {
        public IEnumerable<Exercise> Browse();
    }


    public class ExerciseService : IExerciseService
    {
        private readonly SqlContext _context;

        public ExerciseService(SqlContext context)
        {
            _context = context;
        }

        public IEnumerable<Exercise> Browse()
        {
            var exercises = _context.Exercises.ToList();
            return exercises;
        }
    }
}