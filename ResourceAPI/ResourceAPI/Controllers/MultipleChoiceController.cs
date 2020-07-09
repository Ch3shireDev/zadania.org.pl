using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResourceAPI.Models;

namespace ResourceAPI.Controllers
{
    [Route("api/v1/multiple_choice")]
    [ApiController]
    public class MultipleChoiceController : ControllerBase
    {
        public MultipleChoiceController(ILogger<ProblemsController> logger, SqlContext context)
        {
            Logger = logger;
            Context = context;
        }

        private ILogger<ProblemsController> Logger { get; }
        private SqlContext Context { get; }


        [HttpGet]
        public ActionResult Get()
        {
            var multipleChoiceTests = Context.MultipleChoiceTests.Select(test => test.Id);
            return StatusCode(200, new
            {
                page = 1,
                totalPages = 1,
                multipleChoiceTestLinks = multipleChoiceTests.Select(id => $"/api/v1/multiple_choice/{id}")
            });
        }

        [HttpPost]
        [Authorize]
        public ActionResult Post(MultipleChoiceTest multipleChoiceTest)
        {
            multipleChoiceTest.Id = 0;
            Context.MultipleChoiceTests.Add(multipleChoiceTest);
            Context.SaveChanges();
            return StatusCode(201);
        }


        [Route("{testId:int}")]
        [HttpGet]
        public ActionResult GetTest(int testId)
        {
            var test = Context.MultipleChoiceTests.Select(t => new MultipleChoiceTest
            {
                Id = t.Id,
                AuthorId = t.AuthorId,
                QuestionLinks = t.Questions.Select(q => $"/api/v1/multiple_choice/{testId}/questions/{q.Id}")
            }).FirstOrDefault(t => t.Id == testId);
            if (test == null) return StatusCode(404);
            return StatusCode(200, test);
        }

        [Route("{testId:int}")]
        [HttpPut]
        [Authorize]
        public ActionResult PutTest(int testId, MultipleChoiceTest multipleChoiceTest)
        {
            multipleChoiceTest.Id = testId;
            var entity = Context.Find<MultipleChoiceTest>(testId);
            Context.Entry(entity).CurrentValues.SetValues(multipleChoiceTest);
            Context.SaveChanges();
            return StatusCode(201);
        }

        [Route("{testId:int}")]
        [HttpDelete]
        [Authorize]
        public ActionResult DeleteTest(int testId)
        {
            var entity = Context.MultipleChoiceTests.FirstOrDefault(t => t.Id == testId);
            if (entity == null) return StatusCode(404);
            Context.MultipleChoiceTests.Remove(entity);
            Context.SaveChanges();
            return StatusCode(200);
        }

        [Route("{testId:int}/questions/{questionId:int}")]
        [HttpGet]
        public ActionResult GetQuestion(int testId, int questionId)
        {
            var question = Context.MultipleChoiceQuestions
                .Select(q => new MultipleChoiceQuestion
                {
                    Id = q.Id,
                    AuthorId = q.AuthorId,
                    TestId = testId,
                    AnswerLinks = q.Answers.Select(a =>
                        $"/api/v1/multiple_choice/{testId}/questions/{questionId}/answers/{a.Id}")
                })
                .FirstOrDefault(q => q.TestId == testId && q.Id == questionId);
            if (question == null) return StatusCode(404);
            return StatusCode(200);
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
            return StatusCode(200);
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