using System.Collections.Generic;
using CommonLibrary;

namespace ExerciseLibrary
{
    public class Exercise : Post
    {
        public string Name { get; set; }
        public List<Script> Scripts { get; set; }

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