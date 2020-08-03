using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace QuizLibrary
{
    [Route("api/v1/quizzes")]
    [ApiController]
    public class QuizzesController : ControllerBase
    {
        private readonly ILogger<QuizzesController> _logger;
        private readonly IQuizService _quizService;

        public QuizzesController(ILogger<QuizzesController> logger,
            IQuizService quizService)
        {
            _logger = logger;
            _quizService = quizService;
        }


        [HttpGet]
        public ActionResult Browse()
        {
            var tests = _quizService.Browse();
            return Ok(tests);
        }

        [HttpPost]
        public ActionResult Post(QuizUserModel test)
        {
            var id = _quizService.Create(test.ToModel());
            return Ok(new Quiz {Id = id});
        }


        [HttpGet("{testId}")]
        public ActionResult GetTest(int testId)
        {
            var test = _quizService.GetTest(testId);
            if (test == null) return Forbid();
            return Ok(test.ToView());
        }

        [HttpPut("{testId}")]
        [Authorize]
        public ActionResult PutTest(int testId, Quiz quiz)
        {
            var result = _quizService.EditTest(testId, quiz);
            if (result == false) return Forbid();
            return Ok();
        }

        [HttpDelete("{testId}")]
        [Authorize]
        public ActionResult DeleteTest(int testId)
        {
            var result = _quizService.DeleteTest(testId);
            if (result == false) return Forbid();
            return Ok();
        }

        [Route("{testId}/questions/{questionId}")]
        [HttpGet]
        public ActionResult GetQuestion(int testId, int questionId)
        {
            var question = _quizService.GetQuestion(questionId);
            if (question == null) return StatusCode(404);
            return Ok(question);
        }

        [HttpPost("{testId}/questions")]
        public ActionResult PostQuestion(int testId, QuizQuestion quizQuestion)
        {
            var id = _quizService.CreateQuestion(testId, quizQuestion);
            if (id == 0) return Forbid();
            return Ok(new QuizQuestion {Id = id, TestId = testId});
        }

        [Route("{testId}/questions/{questionId}")]
        [HttpDelete]
        public ActionResult DeleteQuestion(int testId, int questionId)
        {
            var result = _quizService.DeleteQuestion(questionId);
            if (result == false) return Forbid();
            return Ok();
        }

        [Route("{testId}/questions/{questionId}")]
        [HttpPut]
        public ActionResult PutQuestion(int testId, int questionId, QuizQuestion quizQuestion)
        {
            var result = _quizService.EditQuestion(questionId, quizQuestion);
            if (result == false) return Forbid();
            return Ok();
        }

        [Route("{testId}/questions/{questionId}/answers/{answerId}")]
        [HttpGet]
        public ActionResult GetAnswer(int testId, int questionId, int answerId)
        {
            var answer = _quizService.GetAnswer(answerId);
            if (answer == null) return NotFound();
            return Ok(answer);
        }

        [Route("{testId}/questions/{questionId}/answers")]
        [HttpPost]
        public ActionResult PostAnswer(int testId, int questionId, QuizAnswer quizAnswer)
        {
            var answerId = _quizService.CreateAnswer(questionId, quizAnswer);
            if (answerId == 0) return Forbid();
            return Ok(new QuizAnswer {Id = answerId, QuestionId = questionId});
        }

        [Route("{testId}/questions/{questionId}/answers/{answerId}")]
        [HttpPut]
        public ActionResult PutAnswer(int testId, int questionId, int answerId,
            QuizAnswer QuizAnswer)
        {
            var result = _quizService.EditAnswer(answerId, QuizAnswer);
            if (result == false) return Forbid();
            return Ok();
        }

        [Route("{testId}/questions/{questionId}/answers/{answerId}")]
        [HttpDelete]
        public ActionResult DeleteAnswer(int testId, int questionId, int answerId)
        {
            var result = _quizService.DeleteAnswer(answerId);
            if (result == false) Forbid();
            return Ok();
        }
    }
}