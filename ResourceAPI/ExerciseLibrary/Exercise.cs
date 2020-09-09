using System.Collections.Generic;
using CommonLibrary;

namespace ExerciseLibrary
{
    public class Exercise : Post
    {
        public string Name { get; set; }
        public List<ExerciseVariableData> VariableData { get; set; }
        public List<ExerciseAnswerData> AnswerData { get; set; }

        public ExerciseLink AsLink()
        {
            return new ExerciseLink
            {
                Id = Id,
                Name = Name
            };
        }
    }
}