using System;

namespace cw5.Models
{
    public class EnrollResponse
    {
        public string IndexNumber { get; set; }

        public int Semester { get; set; }

        public string Studies { get; set; }

        public DateTime StartDate { get; set; }

        public int IdEnrollment { get; set; }

        public int IdStudies { get; set; }
    }
}
