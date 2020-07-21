using System.Collections.Generic;
using ResourceAPI.Models.Exercise;

namespace ResourceAPI.ApiServices.Interfaces
{
    public interface IExerciseService
    {
        public IEnumerable<AutomatedExercise> Browse();
    }
}