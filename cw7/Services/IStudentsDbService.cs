using cw3.Models;
using cw5.Models;
using System;
using System.Collections.Generic;

namespace cw5.Services
{
    public interface IStudentsDbService
    {
        EnrollResponse EnrollStudent(EnrollRequest request);

        PromoteResponse PromoteStudents(PromoteRequest request);

        Student GetStudent(int id);

        IEnumerable<Student> GetStudents();

        Boolean Validate(String index);

    }
}
