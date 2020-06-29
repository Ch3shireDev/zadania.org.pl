using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResourceAPI.Models;

namespace ResourceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ILogger<ProblemsController> _logger;

        public TagsController(ILogger<ProblemsController> logger, SqlContext context)
        {
            _logger = logger;
            Context = context;
        }

        private SqlContext Context { get; }

        public static void RefreshTags(Problem problem, SqlContext context)
        {
            if (problem.Tags == null) return;
            var list = new List<Tag>();
            foreach (var tag in problem.Tags)
            {
                var tagElement = context.Tags.FirstOrDefault(element => element.Name == tag.Name);
                if (tagElement == null)
                {
                    tagElement = tag;
                    context.Tags.Add(tagElement);
                }

                tagElement.Parent = problem;
                list.Add(tagElement);
            }

            problem.Tags = list;
        }
    }
}