using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResourceAPI.ApiServices;
using ResourceAPI.Models.Post;

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

        //[HttpPost]
        //[Route("register")]
        //[Authorize]
        //public ActionResult Register(UserData user)
        //{
        //    var idClaim = HttpContext.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier);
        //    var nameIdentifier = idClaim.Value;

        //    if (Context.Authors.Any(profile => profile.UserId == nameIdentifier)) return StatusCode(200);
        //    var newProfile = new Author {UserId = nameIdentifier, Name = user.Name, Email = user.Email};
        //    Context.Authors.Add(newProfile);
        //    Context.SaveChanges();
        //    return StatusCode(201);
        //}

        [HttpGet]
        [Route("self")]
        //[Authorize]
        public ActionResult Get()
        {
            var author = _authorService.GetAuthor(HttpContext);
            if (author != null)
            {
                author.Problems = null;
                author.Answers = null;
                author.VotedProblems = null;
            }

            return StatusCode(200, author);
        }

        [HttpGet]
        [Route("{id}")]
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