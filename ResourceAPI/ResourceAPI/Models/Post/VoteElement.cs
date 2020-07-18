using ResourceAPI.Enums;

namespace ResourceAPI.Models.Post
{
    public class VoteElement
    {
        public int Id { get; set; }
        public int ElementId { get; set; }
        public Vote Vote { get; set; }
    }
}