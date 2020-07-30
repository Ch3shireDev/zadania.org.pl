using System.Collections.Generic;

namespace ExerciseLibrary
{
    public interface IExerciseService
    {
        public IEnumerable<Exercise> Browse();
        int Create(Exercise exercise, int categoryId = 1, int authorId = 1);
        Exercise Get(int exerciseId);
        bool Delete(int exerciseId);
        bool Edit(int exerciseId, Exercise exercise);
        int CreateScript(int exerciseId, Script script);
        Script GetScript(int exerciseId, int scriptId);
        bool EditScript(int exerciseId, int scriptId, Script script);
        bool DeleteScript(int exerciseId, int scriptId);
    }
}