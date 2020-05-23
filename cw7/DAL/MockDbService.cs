﻿using System.Collections.Generic;
using cw3.Models;

namespace cw3.DAL
{
    public class MockDbService : IDbService
    {
        private static IEnumerable<Student> _students;

        static MockDbService()
        {
            _students = new List<Student>
            {
                new Student{IdStudent = 1, firstName = "Jan", lastName = "Kowalski"},
                new Student{IdStudent = 2, firstName = "Anna", lastName = "Malewska"},
                new Student{IdStudent = 3, firstName = "Andrzej", lastName = "andrzejewicz"}
            };
        }
            
        public IEnumerable<Student> getStudents()
        {
            return _students;
        }
    }
}