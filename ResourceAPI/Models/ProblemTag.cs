using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceAPI.Models
{
    public class ProblemTag
    {
        public Problem Problem { get; set; }

        [Key] [Column(Order = 0)] public int ProblemId { get; set; }

        public Tag Tag { get; set; }

        [Key] [Column(Order = 1)] 
        [StringLength(64)]
        public string TagUrl { get; set; }
    }
}