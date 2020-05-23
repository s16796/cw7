using System.ComponentModel.DataAnnotations;

namespace cw5.Models
{
    public class PromoteRequest
    {
        [Required] public string Studies { get; set; }

        [Required] public int Semester { get; set; }
    }
}
