using cw3.Models;
using cw5.Models;
using cw7.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace cw5.Services
{
    public class SqlServerDbService : IStudentsDbService
    {
        public EnrollResponse EnrollStudent(EnrollRequest request)
        {
            EnrollResponse enrollResponse = new EnrollResponse();
            using (var client = new SqlConnection("Data Source = db-mssql.pjwstk.edu.pl; Initial Catalog = s16796; Integrated Security = True"))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    client.Open();
                    var transaction = client.BeginTransaction();
                    command.Transaction = transaction;
                    command.CommandText = "Select IdStudy FROM Studies where Name=@name";
                    command.Parameters.AddWithValue("name", request.Studies);
                    var dr = command.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        transaction.Rollback();
                        throw new ArgumentException("No studies found with that name");
                    }
                    else
                    {
                        enrollResponse.IdStudies = Convert.ToInt32(dr["IdStudy"].ToString());
                        enrollResponse.Semester = 1;
                        enrollResponse.Studies = request.Studies;
                    }
                    dr.Close();
                    command.Parameters.Clear();
                    command.CommandText = "SELECT IdEnrollment, StartDate FROM Enrollment WHERE semester = 1 AND IdStudy = @id order by StartDate desc";
                    command.Parameters.AddWithValue("id", enrollResponse.IdStudies);
                    dr = command.ExecuteReader();
                    if (dr.Read())
                    {

                        enrollResponse.IdEnrollment = Convert.ToInt32(dr["IdEnrollment"].ToString());
                        enrollResponse.StartDate = Convert.ToDateTime(dr["StartDate"]);
                    }
                    else
                    {
                        //enrollment nie istnieje!
                        dr.Close();
                        command.CommandText = "INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate) OUTPUT Inserted.IdEnrollment VALUES((SELECT MAX(IdEnrollment) FROM Enrollment) + 1, 1, @id, @startdate)";
                        command.Parameters.AddWithValue("startdate", DateTime.Now);
                        dr = command.ExecuteReader();
                        dr.Read();
                        enrollResponse.IdEnrollment = Convert.ToInt32(dr["IdEnrollment"].ToString());
                        enrollResponse.StartDate = DateTime.Now.Date;
                    }

                    dr.Close();
                    var studentsalt = GetSalt(32);
                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment, Password, Salt) VALUES(@index, @first, @last, @birth, @enrollment, @Password, @Salt)";
                    command.Parameters.AddWithValue("index", request.IndexNumber);
                    command.Parameters.AddWithValue("first", request.FirstName);
                    command.Parameters.AddWithValue("last", request.LastName);
                    command.Parameters.AddWithValue("birth", DateTime.ParseExact(request.BirthDate, "dd.MM.yyyy", null));
                    command.Parameters.AddWithValue("enrollment", enrollResponse.IdEnrollment);
                    command.Parameters.AddWithValue("Password", PasswordHasherService.GenerateSaltedHash(request.Password, studentsalt));
                    command.Parameters.AddWithValue("Salt", studentsalt);

                    try
                    {
                        dr = command.ExecuteReader();
                        enrollResponse.IndexNumber = request.IndexNumber;
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc);
                        dr.Close();
                        transaction.Rollback();
                        throw new ArgumentException("This index already exists");
                    }

                    dr.Close();
                    transaction.Commit();
                    return enrollResponse;


                }
            }
        }

        public Student GetStudent(int id)
        {
            var student = new Student();
            using (var client = new SqlConnection("Data Source = db-mssql.pjwstk.edu.pl; Initial Catalog = s16796; Integrated Security = True"))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    //indexy od 12 do 20, podawane jako np localhost:44312/api/students/13 da studenta s13
                    command.CommandText = "SELECT * FROM Student st JOIN ENROLLMENT enr ON st.IdEnrollment = enr.IdEnrollment JOIN Studies sts on enr.IdStudy = sts.IdStudy WHERE IndexNumber LIKE '%' + CAST(@id AS varchar)";
                    command.Parameters.AddWithValue("id", id);
                    client.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        student.firstName = reader["FirstName"].ToString();
                        student.lastName = reader["LastName"].ToString();
                        student.BirthDate = Convert.ToDateTime(reader["BirthDate"].ToString());
                        student.indexNumber = reader["IndexNumber"].ToString();
                        student.Study = reader["Name"].ToString();
                        student.Semester = Convert.ToInt32(reader["Semester"].ToString());
                    }
                }
            }
            return student;
        }

        public IEnumerable<Student> GetStudents()
        {
            var listofstudents = new List<Student>();
            using (var client = new SqlConnection("Data Source = db-mssql.pjwstk.edu.pl; Initial Catalog = s16796; Integrated Security = True"))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    command.CommandText = "SELECT * FROM Student st JOIN ENROLLMENT enr ON st.IdEnrollment = enr.IdEnrollment JOIN Studies sts on enr.IdStudy = sts.IdStudy";
                    client.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var st = new Student();
                        st.firstName = reader["FirstName"].ToString();
                        st.lastName = reader["LastName"].ToString();
                        st.BirthDate = Convert.ToDateTime(reader["BirthDate"].ToString());
                        st.indexNumber = reader["IndexNumber"].ToString();
                        st.Study = reader["Name"].ToString();
                        st.Semester = Convert.ToInt32(reader["Semester"].ToString());

                        listofstudents.Add(st);
                    }
                }
            }
            return listofstudents;
        }

        public PromoteResponse PromoteStudents(PromoteRequest request)
        {
            PromoteResponse response = new PromoteResponse();

            using (var client = new SqlConnection("Data Source = db-mssql.pjwstk.edu.pl; Initial Catalog = s16796; Integrated Security = True"))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    client.Open();
                    command.CommandText = "EXEC Promote @Studies, @Semester";
                    command.Parameters.AddWithValue("Studies", request.Studies);
                    command.Parameters.AddWithValue("Semester", request.Semester);
                    var dr = command.ExecuteReader();
                    if (dr.Read())
                    {
                        response.IdEnrollment = Convert.ToInt32(dr["IdEnrollment"].ToString());
                        response.IdStudy = Convert.ToInt32(dr["IdStudy"].ToString());
                        response.Semester = Convert.ToInt32(dr["Semester"].ToString());
                        response.StartDate = Convert.ToDateTime(dr["StartDate"].ToString());

                        return response;
                    }
                    else
                    {
                        throw new ArgumentException("Promotion failed, wrong arguments");
                    }

                }
            }
        }

        public bool Validate(string index)
        {
            using (var client = new SqlConnection("Data Source = db-mssql.pjwstk.edu.pl; Initial Catalog = s16796; Integrated Security = True"))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    client.Open();
                    command.CommandText = "SELECT * FROM Student st WHERE st.IndexNumber Like @Index";
                    command.Parameters.AddWithValue("Index", index);
                    var dr = command.ExecuteReader();
                    if (dr.Read())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        private static byte[] GetSalt(int maximumSaltLength)
        {
            var salt = new byte[maximumSaltLength];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return salt;
        }
    }
}
