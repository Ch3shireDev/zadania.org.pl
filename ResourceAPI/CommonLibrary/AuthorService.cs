using System.Linq;

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
            if (_context.Authors.Any(a => a.UserId == nameIdentifier))
                return _context.Authors.First(a => a.UserId == nameIdentifier).Id;
            var newProfile = new Author
            {
                UserId = nameIdentifier,
                Name = profile.Name,
                Email = profile.Email
            };
            _context.Authors.Add(newProfile);
            _context.SaveChanges();

            return _context.Authors.First(a => a.UserId == nameIdentifier).Id;
        }

        public Author GetAuthor(int id)
        {
            return _context.Authors.FirstOrDefault(a => a.Id == id);
        }
    }
}