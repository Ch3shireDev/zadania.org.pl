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
        [NotMapped] public string Url => $"/api/v1/quiz/{Id}";

        public QuizLink ToLink()
        {
            return new QuizLink
            {
                Id = Id,
                Name = Name
            };
        }

        public QuizView ToView()
        {
            return new QuizView
            {
                Id = Id,
                Name = Name,
                Content = Tools.Render(Content, Files)
            };
        }
    }
}