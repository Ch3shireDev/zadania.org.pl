using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ResourceAPI.ApiServices.Interfaces;
using ResourceAPI.Models.Post;
using ResourceAPI.Tools;

namespace ResourceAPI.ApiServices
{
    public class AuthorService : IAuthorService
    {
        private readonly SqlContext _context;

        public AuthorService(SqlContext context)
        {
            _context = context;
        }

        public Author GetAuthor(HttpContext context)
        {
#if DEBUG
            if (!_context.Authors.Any()) _context.Authors.Add(new Author {Name = "dummy", UserId = "dummy"});
            _context.SaveChanges();
            return _context.Authors.First();
#endif

            var profileData = context.Request.Headers["profile"][0];
            var profile = JsonConvert.DeserializeObject<UserData>(profileData);
            //return _authorService.GetAuthor(, profile);

            var http_contextUser = context.User;
            var idClaim = http_contextUser.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            if (idClaim == null) return null;
            var nameIdentifier = idClaim.Value;

            if (!_context.Authors.Any(profile => profile.UserId == nameIdentifier))
            {
                var newProfile = new Author {UserId = nameIdentifier, Name = profile.Name, Email = profile.Email};
                _context.Authors.Add(newProfile);
                _context.SaveChanges();
            }

            return _context.Authors.First(profile => profile.UserId == nameIdentifier);
        }
    }
}