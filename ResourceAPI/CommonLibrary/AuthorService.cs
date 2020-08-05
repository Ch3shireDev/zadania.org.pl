using System.Linq;
using CommonLibrary.Interfaces;

namespace CommonLibrary
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorDbContext _context;

        public AuthorService(IAuthorDbContext context)
        {
            _context = context;
            if (!_context.Authors.Any()) _context.Authors.Add(new Author {Name = "Admininstrator", UserId = "admin"});
            _context.SaveChanges();
        }

        public int GetAuthor(string nameIdentifier, UserData profile)
        {
            //var profileData = context.Request.Headers["profile"][0];
            //var profile = JsonConvert.DeserializeObject<UserData>(profileData);
            //return _authorService.GetAuthor(, profile);

            //var httpContextUser = context.User;
            //var idClaim = httpContextUser.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            //if (idClaim == null) return null;
            //var nameIdentifier = idClaim.Value;

            if (!_context.Authors.Any(a => a.UserId == nameIdentifier))
            {
                var newProfile = new Author
                {
                    UserId = nameIdentifier,
                    Name = profile.Name,
                    Email = profile.Email
                };
                _context.Authors.Add(newProfile);
                _context.SaveChanges();
            }

            return _context.Authors.First(a => a.UserId == nameIdentifier).Id;
        }

        public Author GetAuthor(int id)
        {
            return _context.Authors.FirstOrDefault(a => a.Id == id);
        }
    }
}