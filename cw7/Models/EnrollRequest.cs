using System.ComponentModel.DataAnnotations;

namespace cw5.Models
{
    public class EnrollRequest
    {
        [Required] public string IndexNumber { get; set; }

        [Required] public string FirstName { get; set; }

        [Required] public string LastName { get; set; }

        [Required] public string BirthDate { get; set; }

        [Required] public string Studies { get; set; }

        [Required] public string Password { get; set; }
    }
}
