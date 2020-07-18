using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResourceAPI.ApiServices.Interfaces;
using ResourceAPI.Models.MultipleChoice;

namespace ResourceAPI.Controllers
{
    [Route("api/v1/multiple-choice")]
    [ApiController]
    public class MultipleChoiceController : ControllerBase
    {
        public MultipleChoiceController(ILogger<ProblemsController> logger, SqlContext context,
            IMultipleChoiceService multipleChoiceService)
        {
            Logger = logger;
            Context = context;
            MultipleChoiceService = multipleChoiceService;
        }

        private ILogger<ProblemsController> Logger { get; }
        private SqlContext Context { get; }
        private IMultipleChoiceService MultipleChoiceService { get; }

        [HttpGet]
        public ActionResult Get()
        {
            var multipleChoiceTests = Context.MultipleChoiceTests.Select(test => test.Id);
            return StatusCode(200, new
            {
                page = 1,
                totalPages = 1,
                multipleChoiceTestLinks = multipleChoiceTests.Select(id => $"/api/v1/multiple-choice/{id}")
            });
        }

        [Route("{testId:int}")]
        [HttpGet]
        public ActionResult GetTest(int testId)
        {
            var test = MultipleChoiceService.GetTestById(testId, true, true);
            return StatusCode(200, test);
        }

        //[Route("{testId:int}")]
        //[HttpPut]
        //[Authorize]
        //public ActionResult PutTest(int testId, MultipleChoiceTest multipleChoiceTest)
        //{
        //    multipleChoiceTest.Id = testId;
        //    var entity = _context.Find<MultipleChoiceTest>(testId);
        //    _context.Entry(entity).CurrentValues.SetValues(multipleChoiceTest);
        //    _context.SaveChanges();
        //    return StatusCode(201);
        //}

        //[Route("{testId:int}")]
        //[HttpDelete]
        //[Authorize]
        //public ActionResult DeleteTest(int testId)
        //{
        //    var entity = _context.MultipleChoiceTests.FirstOrDefault(t => t.Id == testId);
        //    if (entity == null) return StatusCode(404);
        //    _context.MultipleChoiceTests.Remove(entity);
        //    _context.SaveChanges();
        //    return StatusCode(200);
        //}

        [Route("{testId:int}/questions/{questionId:int}")]
        [HttpGet]
        public ActionResult GetQuestion(int testId, int questionId)
        {
            var question = MultipleChoiceService.GetQuestionById(testId, questionId, true);
            if (question == null) return StatusCode(404);
            return StatusCode(200, question);
        }

        [Route("{testId:int}")]
        [HttpPost]
        public ActionResult PostQuestion(int testId, MultipleChoiceQuestion multipleChoiceQuestion)
        {
            return StatusCode(201);
        }

        [Route("{testId:int}/questions/{questionId:int}")]
        [HttpDelete]
        public ActionResult DeleteQuestion(int testId, int questionId)
        {
            return StatusCode(200);
        }

        [Route("{testId:int}/questions/{questionId:int}")]
        [HttpPut]
        public ActionResult PutQuestion(int testId, int questionId, MultipleChoiceQuestion multipleChoiceQuestion)
        {
            return StatusCode(200);
        }

        [Route("{testId:int}/questions/{questionId:int}/answers/{answerId:int}")]
        [HttpGet]
        public ActionResult GetAnswer(int testId, int questionId, int answerId)
        {
            var answer = MultipleChoiceService.GetAnswerById(testId, questionId, answerId);
            if (answer == null) return StatusCode(404);
            return StatusCode(200, answer);
        }

        [Route("{testId:int}/questions/{questionId:int}/answers")]
        [HttpPost]
        public ActionResult PostAnswer(int testId, int questionId, MultipleChoiceAnswer multipleChoiceAnswer)
        {
            return StatusCode(200);
        }

        [Route("{testId:int}/questions/{questionId:int}/answers/{answerId:int}")]
        [HttpPut]
        public ActionResult PutAnswer(int testId, int questionId, int answerId,
            MultipleChoiceAnswer multipleChoiceAnswer)
        {
            return StatusCode(200);
        }

        [Route("{testId:int}/questions/{questionId:int}/answers/{answerId:int}")]
        [HttpDelete]
        public ActionResult DeleteAnswer(int testId, int questionId, int answerId)
        {
            return StatusCode(200);
        }
    }
}