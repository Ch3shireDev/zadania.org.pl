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
        int CreateVariableData(int exerciseId, ExerciseVariableData exerciseVariableData);
        ExerciseVariableData GetVariableData(int exerciseId, int scriptId);
        bool EditVariableData(int exerciseId, int scriptId, ExerciseVariableData exerciseVariableData);
        bool DeleteVariableData(int exerciseId, int scriptId);
    }
}