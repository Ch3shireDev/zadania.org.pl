using System.Linq;
using System.Security.Claims;
using CommonLibrary;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ProblemLibrary;
using ResourceAPI.ApiServices.Interfaces;

namespace ResourceAPI.ApiServices
{
    public class AuthorService : IAuthorService
    {
        private readonly SqlContext _context;

        public AuthorService(SqlContext context)
        {
            _context = context;
            if (!_context.Authors.Any()) _context.Authors.Add(new Author {Name = "Admininstrator", UserId = "admin"});
            _context.SaveChanges();
        }

        public Author GetAuthor(int id)
        {
            return _context.Authors.FirstOrDefault(a => a.Id == id);
        }

        public Author GetAuthor(HttpContext context)
        {
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