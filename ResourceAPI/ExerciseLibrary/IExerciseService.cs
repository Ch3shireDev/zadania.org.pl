using System.Collections.Generic;
using ExerciseLibrary;

namespace ResourceAPI.ApiServices.Interfaces
{
    public interface IExerciseService
    {
        public IEnumerable<Exercise> Browse();
        int Create(Exercise exercise, int authorId = 1);
    }
}