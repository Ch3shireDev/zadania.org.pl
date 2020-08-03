using Microsoft.EntityFrameworkCore;

namespace VoteLibrary
{
    public interface IVoteDbContext
    {
        public DbSet<VoteElement> Votes { get; set; }
    }
}