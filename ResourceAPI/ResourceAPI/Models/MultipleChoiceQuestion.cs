//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace ResourceAPI.Models
//{
//    public class MultipleChoiceQuestion : Post
//    {
//        //public List<MultipleChoiceAnswer> Answers { get; set; }
//        public MultipleChoiceTest Test { get; set; }
//        public int TestId { get; set; }
//        [NotMapped] public IEnumerable<string> AnswerLinks { get; set; }
//    }

//    public class MultipleChoiceAnswer:Post
//    {
//        public bool IsCorrectAnswer { get; set; }
//        public MultipleChoiceQuestion Question { get; set; }
//        public int QuestionId { get; set; }
//    }
//}