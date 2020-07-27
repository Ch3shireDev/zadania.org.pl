using System.Collections.Generic;
using ResourceAPI.Models.Exercise;

namespace ResourceAPI.ApiServices.Interfaces
{
    public interface IExerciseService
    {
        public IEnumerable<Exercise> Browse();
        int Create(Exercise exercise, int authorId = 1);
    }
}