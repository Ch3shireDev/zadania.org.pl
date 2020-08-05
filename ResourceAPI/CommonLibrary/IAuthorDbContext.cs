using Microsoft.EntityFrameworkCore;

namespace CommonLibrary.Interfaces
{
    public interface IAuthorDbContext
    {
        DbSet<Author> Authors { get; set; }
        int SaveChanges(bool acceptAllChangesOnSuccess = true);
    }
}