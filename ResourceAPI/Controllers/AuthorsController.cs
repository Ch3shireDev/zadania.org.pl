﻿using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ResourceAPI.Models;

namespace ResourceAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ILogger<ProblemsController> _logger;

        public AuthorsController(ILogger<ProblemsController> logger, DatabaseContext context)
        {
            _logger = logger;
            Context = context;
        }

        private DatabaseContext Context { get; }

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
            var author = GetAuthor(HttpContext, Context);
            author.Problems = null;
            author.Answers = null;
            author.VotedProblems = null;
            return StatusCode(200, author);
        }

        [HttpGet]
        [Route("{userId}")]
        public ActionResult Get(int userId)
        {
            if (!Context.Authors.Any(p => p.Id == userId)) return StatusCode(404);
            var profile = Context.Authors.First(p => p.Id == userId);
            profile.UserId = null;
            return StatusCode(200, profile);
        }

        public static Author GetAuthor(HttpContext httpContext, DatabaseContext context)
        {
            return null;
            var httpContextUser = httpContext.User;
            var idClaim = httpContextUser.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier);
            var nameIdentifier = idClaim.Value;

            if (!context.Authors.Any(profile => profile.UserId == nameIdentifier))
            {
                var profileData = httpContext.Request.Headers["profile"][0];
                var profile = JsonConvert.DeserializeObject<UserData>(profileData);
                var newProfile = new Author {UserId = nameIdentifier, Name = profile.Name, Email = profile.Email};
                context.Authors.Add(newProfile);
                context.SaveChanges();
            }

            return context.Authors.First(profile => profile.UserId == nameIdentifier);
        }
    }

    public class UserData
    {
        public string Nickname { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}