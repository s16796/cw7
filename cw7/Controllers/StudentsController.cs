using System;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;
using cw5.Services;
using Microsoft.Extensions.Configuration;
using cw7.Services;
using Microsoft.AspNetCore.Authorization;
using cw7.Models;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/students")]

    public class StudentsController : ControllerBase
    {

        private readonly IStudentsDbService _dbService;
        private IJWTauthService _JWTauthService;

        public StudentsController(IStudentsDbService dbservice, IConfiguration configuration, IJWTauthService JWTauthService)
        {
            _dbService = dbservice;
            _JWTauthService = JWTauthService;
        }

        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            return Ok(_dbService.GetStudent(id));
        }

        [HttpGet]
        public IActionResult GetStudents(string orderBy)
        {
            return Ok(_dbService.GetStudents());
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.indexNumber = $"s{new Random().Next(1, 99999)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult PutStudent(int id, Student student)
        {
            return Ok("Aktualizacja studenta nr " + id + " dokończona.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Usuwanie studenta nr " + id + " ukończone.");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public IActionResult Login(LoginRequest request)
        {
            var token = _JWTauthService.Login(request);
            if (token != null)
            {
                return Ok(token);
            }
            else
            {
                return Unauthorized("Wrong login or password entered");
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("refreshToken")]
        public IActionResult RefreshToken(RefreshRequest request)
        {
            var token = _JWTauthService.RefreshToken(request);
            if (token != null)
            {
                return Ok(token);
            }
            else
            {
                return NotFound("No token like that found in database");
            }
        }

    }
}