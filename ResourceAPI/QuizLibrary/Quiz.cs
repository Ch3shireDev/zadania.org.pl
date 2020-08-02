using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CommonLibrary;

namespace QuizLibrary
{
    public class Quiz : Post
    {
        public string Name { get; set; }
        public List<QuizQuestion> Questions { get; set; }
        public bool CanBeRandomized { get; set; }
        [NotMapped] public string Url => $"/api/v1/quizzes/{Id}";

        public QuizLink AsLink()
        {
            return new QuizLink
            {
                Id = Id,
                Url = Url,
                Name = Name
            };
        }
    }
}