using System.Linq;
using CommonLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProblemLibrary;
using ResourceAPI.ApiServices.Interfaces;

namespace ResourceAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        private readonly ILogger<ProblemsController> _logger;

        public AuthorsController(ILogger<ProblemsController> logger, SqlContext context, IAuthorService authorService)
        {
            _logger = logger;
            Context = context;
            _authorService = authorService;
        }

        private SqlContext Context { get; }


        [HttpGet("self")]
        [Authorize]
        public ActionResult Get()
        {
            var author = _authorService.GetAuthor(1);
            if (author != null)
            {
                //author.Problems = null;
                //author.Answers = null;
                author.VotedProblems = null;
            }

            return StatusCode(200, author);
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var author = Context.Authors.Select(a => new Author
                {
                    Id = a.Id,
                    Name = a.Name,
                    UserId = a.UserId,
                    Email = a.Email
                })
                .FirstOrDefault(a => a.Id == id);
            if (author == null) return StatusCode(404);
            return StatusCode(200, author);
        }
    }
}