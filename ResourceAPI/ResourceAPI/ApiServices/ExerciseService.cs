using System.Collections.Generic;
using System.Linq;
using ResourceAPI.ApiServices.Interfaces;
using ResourceAPI.Models.Exercise;

namespace ResourceAPI.ApiServices
{
    public class ExerciseService : IExerciseService
    {
        private readonly SqlContext _context;

        public ExerciseService(SqlContext context)
        {
            _context = context;
        }

        public IEnumerable<AutomatedExercise> Browse()
        {
            var exercises = _context.Exercises.ToList();
            return exercises;
        }
    }
}