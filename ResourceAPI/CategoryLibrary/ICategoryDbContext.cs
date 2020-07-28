using Microsoft.EntityFrameworkCore;

namespace CategoryLibrary
{
    public interface ICategoryDbContext
    {
        public DbSet<Category> Categories { get; set; }
        int SaveChanges(bool acceptAllChangesOnSuccess = true);
    }
}