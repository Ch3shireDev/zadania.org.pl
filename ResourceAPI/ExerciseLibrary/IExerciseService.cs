using System.Collections.Generic;

namespace ExerciseLibrary
{
    public interface IExerciseService
    {
        public IEnumerable<Exercise> Browse();
        int Create(Exercise exercise, int authorId = 1);
    }
}