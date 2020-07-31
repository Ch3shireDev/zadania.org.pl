using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace QuizLibrary
{
    [Route("api/v1/quiz")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        private ILogger<QuizController> _logger;

        public QuizController(ILogger<QuizController> logger,
            IQuizService QuizService)
        {
            _logger = logger;
            _quizService = QuizService;
        }


        [HttpGet]
        public ActionResult Browse()
        {
            var tests = _quizService.Browse();
            return Ok(tests);
        }

        [HttpPost]
        public ActionResult Post(Quiz test)
        {
            var id = _quizService.Create(test);
            return Ok(new Quiz {Id = id});
        }


        [HttpGet("{testId:int}")]
        public ActionResult GetTest(int testId)
        {
            var test = _quizService.GetTest(testId);
            if (test == null) return Forbid();
            return Ok(test);
        }

        [HttpPut("{testId:int}")]
        [Authorize]
        public ActionResult PutTest(int testId, Quiz quiz)
        {
            var result = _quizService.EditTest(testId, quiz);
            if (result == false) return Forbid();
            return Ok();
        }

        [HttpDelete("{testId:int}")]
        [Authorize]
        public ActionResult DeleteTest(int testId)
        {
            var result = _quizService.DeleteTest(testId);
            if (result == false) return Forbid();
            return Ok();
        }

        [Route("{testId:int}/questions/{questionId:int}")]
        [HttpGet]
        public ActionResult GetQuestion(int testId, int questionId)
        {
            var question = _quizService.GetQuestion(questionId);
            if (question == null) return StatusCode(404);
            return Ok(question);
        }

        [HttpPost("{testId:int}/questions")]
        public ActionResult PostQuestion(int testId, QuizQuestion QuizQuestion)
        {
            var id = _quizService.CreateQuestion(testId, QuizQuestion);
            if (id == 0) return Forbid();
            return Ok(new QuizQuestion {Id = id, TestId = testId});
        }

        [Route("{testId:int}/questions/{questionId:int}")]
        [HttpDelete]
        public ActionResult DeleteQuestion(int testId, int questionId)
        {
            var result = _quizService.DeleteQuestion(questionId);
            if (result == false) return Forbid();
            return Ok();
        }

        [Route("{testId:int}/questions/{questionId:int}")]
        [HttpPut]
        public ActionResult PutQuestion(int testId, int questionId, QuizQuestion QuizQuestion)
        {
            var result = _quizService.EditQuestion(questionId, QuizQuestion);
            if (result == false) return Forbid();
            return Ok();
        }

        [Route("{testId:int}/questions/{questionId:int}/answers/{answerId:int}")]
        [HttpGet]
        public ActionResult GetAnswer(int testId, int questionId, int answerId)
        {
            var answer = _quizService.GetAnswer(answerId);
            if (answer == null) return NotFound();
            return Ok(answer);
        }

        [Route("{testId:int}/questions/{questionId:int}/answers")]
        [HttpPost]
        public ActionResult PostAnswer(int testId, int questionId, QuizAnswer QuizAnswer)
        {
            var answerId = _quizService.CreateAnswer(questionId, QuizAnswer);
            if (answerId == 0) return Forbid();
            return Ok(new QuizAnswer {Id = answerId, QuestionId = questionId});
        }

        [Route("{testId:int}/questions/{questionId:int}/answers/{answerId:int}")]
        [HttpPut]
        public ActionResult PutAnswer(int testId, int questionId, int answerId,
            QuizAnswer QuizAnswer)
        {
            var result = _quizService.EditAnswer(answerId, QuizAnswer);
            if (result == false) return Forbid();
            return Ok();
        }

        [Route("{testId:int}/questions/{questionId:int}/answers/{answerId:int}")]
        [HttpDelete]
        public ActionResult DeleteAnswer(int testId, int questionId, int answerId)
        {
            var result = _quizService.DeleteAnswer(answerId);
            if (result == false) Forbid();
            return Ok();
        }
    }
}