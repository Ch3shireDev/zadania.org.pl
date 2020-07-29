using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MultipleChoiceLibrary
{
    [Route("api/v1/multiple-choice-tests")]
    [ApiController]
    public class MultipleChoiceController : ControllerBase
    {
        private readonly IMultipleChoiceService _multipleChoiceService;

        private ILogger<MultipleChoiceController> _logger;

        public MultipleChoiceController(ILogger<MultipleChoiceController> logger,
            IMultipleChoiceService multipleChoiceService)
        {
            _logger = logger;
            _multipleChoiceService = multipleChoiceService;
        }


        [HttpGet]
        public ActionResult Browse()
        {
            var tests = _multipleChoiceService.Browse();
            return Ok(tests);
        }

        [HttpPost]
        public ActionResult Post(MultipleChoiceTest test)
        {
            var id = _multipleChoiceService.Create(test);
            return Ok(new MultipleChoiceTest {Id = id});
        }


        [HttpGet("{testId:int}")]
        public ActionResult GetTest(int testId)
        {
            var test = _multipleChoiceService.GetTest(testId);
            if (test == null) return Forbid();
            return Ok(test);
        }

        [HttpPut("{testId:int}")]
        [Authorize]
        public ActionResult PutTest(int testId, MultipleChoiceTest multipleChoiceTest)
        {
            var result = _multipleChoiceService.EditTest(testId, multipleChoiceTest);
            if (result == false) return Forbid();
            return Ok();
        }

        [HttpDelete("{testId:int}")]
        [Authorize]
        public ActionResult DeleteTest(int testId)
        {
            var result = _multipleChoiceService.DeleteTest(testId);
            if (result == false) return Forbid();
            return Ok();
        }

        [Route("{testId:int}/questions/{questionId:int}")]
        [HttpGet]
        public ActionResult GetQuestion(int testId, int questionId)
        {
            var question = _multipleChoiceService.GetQuestion(questionId);
            if (question == null) return StatusCode(404);
            return Ok(question);
        }

        [HttpPost("{testId:int}/questions")]
        public ActionResult PostQuestion(int testId, MultipleChoiceQuestion multipleChoiceQuestion)
        {
            var id = _multipleChoiceService.CreateQuestion(testId, multipleChoiceQuestion);
            if (id == 0) return Forbid();
            return Ok(new MultipleChoiceQuestion {Id = id, TestId = testId});
        }

        [Route("{testId:int}/questions/{questionId:int}")]
        [HttpDelete]
        public ActionResult DeleteQuestion(int testId, int questionId)
        {
            var result = _multipleChoiceService.DeleteQuestion(questionId);
            if (result == false) return Forbid();
            return Ok();
        }

        [Route("{testId:int}/questions/{questionId:int}")]
        [HttpPut]
        public ActionResult PutQuestion(int testId, int questionId, MultipleChoiceQuestion multipleChoiceQuestion)
        {
            var result = _multipleChoiceService.EditQuestion(questionId, multipleChoiceQuestion);
            if (result == false) return Forbid();
            return Ok();
        }

        [Route("{testId:int}/questions/{questionId:int}/answers/{answerId:int}")]
        [HttpGet]
        public ActionResult GetAnswer(int testId, int questionId, int answerId)
        {
            var answer = _multipleChoiceService.GetAnswer(answerId);
            if (answer == null) return NotFound();
            return Ok(answer);
        }

        [Route("{testId:int}/questions/{questionId:int}/answers")]
        [HttpPost]
        public ActionResult PostAnswer(int testId, int questionId, MultipleChoiceAnswer multipleChoiceAnswer)
        {
            var answerId = _multipleChoiceService.CreateAnswer(questionId, multipleChoiceAnswer);
            if (answerId == 0) return Forbid();
            return Ok(new MultipleChoiceAnswer {Id = answerId, QuestionId = questionId});
        }

        [Route("{testId:int}/questions/{questionId:int}/answers/{answerId:int}")]
        [HttpPut]
        public ActionResult PutAnswer(int testId, int questionId, int answerId,
            MultipleChoiceAnswer multipleChoiceAnswer)
        {
            var result = _multipleChoiceService.EditAnswer(answerId, multipleChoiceAnswer);
            if (result == false) return Forbid();
            return Ok();
        }

        [Route("{testId:int}/questions/{questionId:int}/answers/{answerId:int}")]
        [HttpDelete]
        public ActionResult DeleteAnswer(int testId, int questionId, int answerId)
        {
            var result = _multipleChoiceService.DeleteAnswer(answerId);
            if (result == false) Forbid();
            return Ok();
        }
    }
}