﻿using System.Collections.Generic;
using cw3.Models;

namespace cw3.DAL
{
    public interface IDbService
    {
        IEnumerable<Student> getStudents();
    }
}