using System.Collections.Generic;
using System.Linq;

namespace ExerciseLibrary
{
    public class ExerciseService : IExerciseService
    {
        private readonly IExerciseDbContext _context;

        public ExerciseService(IExerciseDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Exercise> Browse()
        {
            var exercises = _context.Exercises.ToList();
            return exercises;
        }

        public int Create(Exercise exercise, int authorId = 1)
        {
            var element = new Exercise
            {
                Name = exercise.Name,
                Content = exercise.Content,
                AuthorId = authorId,
                CategoryId = exercise.CategoryId
            };
            _context.Exercises.Add(element);
            _context.SaveChanges();
            return element.Id;
        }
    }
}