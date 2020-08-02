using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CommonLibrary;

namespace ExerciseLibrary
{
    public class Exercise : Post
    {
        public string Name { get; set; }
        public List<Script> Scripts { get; set; }
        [NotMapped] public string Url => $"/api/v1/exercises/{Id}";

        public ExerciseLink AsLink()
        {
            return new ExerciseLink
            {
                Id = Id,
                Name = Name,
                Url = Url
            };
        }
    }
}